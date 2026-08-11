using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Genocs.BarcodeLibrary.WebApi.OpenApi;

/// <summary>
/// Applies operation summary, description, and tags to Genocs WebApi endpoints.
/// Genocs.WebApi.OpenApi builds operations from <c>WebApiEndpointDefinitions</c> without descriptions,
/// so this filter enriches the generated document after that metadata is patched in.
/// </summary>
internal sealed class EndpointDescriptionsDocumentFilter : IDocumentFilter
{
    private static readonly EndpointDoc[] Docs =
    [
        new(
            Path: "/health",
            Method: HttpMethod.Get,
            Summary: "Health check",
            Description:
                "Returns a lightweight liveness payload for the Genocs.BarcodeLibrary.WebApi host. " +
                "Use this endpoint for container/orchestrator probes and basic connectivity checks.",
            Tags: ["System"]),

        new(
            Path: "/api/pdf",
            Method: HttpMethod.Post,
            Summary: "Build PDF",
            Description:
                "Builds a PDF from an XSLT template and printable model. " +
                "The request body selects the template (`templateId`), supplies the document `model`, " +
                "and optionally overrides localization with `countryId`. " +
                "On success the response body is `application/pdf` and the persisted job id is returned " +
                "in the `X-Pdf-Job-Id` response header. Job metadata is stored in MongoDB (`pdf_jobs`).",
            Tags: ["PDF"]),

        new(
            Path: "/api/pdf/jobs",
            Method: HttpMethod.Get,
            Summary: "List processed PDF jobs",
            Description:
                "Returns a paginated list of PDF jobs from MongoDB (`pdf_jobs`) via Genocs.Persistence.MongoDB. " +
                "Paging uses zero-based `page` and page size `results` (max 100). " +
                "When `status` is omitted, only completed (processed) jobs are returned; " +
                "pass `status=all` (or `*`) to include every status. " +
                "Optional `templateId`, `orderBy`, and `sortOrder` filters/sorting are supported. " +
                "Default sort is `CreatedAtUtc` descending.",
            Tags: ["PDF"])
    ];

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        foreach (var doc in Docs)
        {
            if (!TryGetPathItem(swaggerDoc, doc.Path, out var pathItem) || pathItem is null)
            {
                continue;
            }

            if (!TryGetOperation(pathItem, doc.Method, out var operation) || operation is null)
            {
                continue;
            }

            operation.Summary = doc.Summary;
            operation.Description = doc.Description;
            operation.Tags ??= new HashSet<OpenApiTagReference>();

            foreach (string tag in doc.Tags)
            {
                if (operation.Tags.Any(existing => string.Equals(existing.Name, tag, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                operation.Tags.Add(new OpenApiTagReference(tag));
            }
        }
    }

    private static bool TryGetPathItem(OpenApiDocument swaggerDoc, string path, out IOpenApiPathItem? pathItem)
    {
        if (swaggerDoc.Paths is not null && swaggerDoc.Paths.TryGetValue(path, out pathItem))
        {
            return true;
        }

        // Genocs stores routes without a leading slash; tolerate either form.
        string alternate = path.StartsWith('/') ? path[1..] : $"/{path}";
        if (swaggerDoc.Paths is not null && swaggerDoc.Paths.TryGetValue(alternate, out pathItem))
        {
            return true;
        }

        pathItem = null;
        return false;
    }

    private static bool TryGetOperation(IOpenApiPathItem pathItem, HttpMethod method, out OpenApiOperation? operation)
    {
        if (pathItem.Operations is not null &&
            pathItem.Operations.TryGetValue(method, out var existing) &&
            existing is OpenApiOperation concrete)
        {
            operation = concrete;
            return true;
        }

        operation = null;
        return false;
    }

    private sealed record EndpointDoc(string Path, HttpMethod Method, string Summary, string Description, string[] Tags);
}
