build:
	dotnet build
start:
	dotnet run --project ./src/WebApi
nuget:
	nuget pack -NoDefaultExcludes -OutputDirectory nupkgs
publish:
	dotnet publish --os linux --arch x64 -c Release --self-contained
publish-to-hub:
	dotnet publish --os linux --arch x64 -c Release -p:ContainerRegistry=docker.io -p:ContainerImageName=genocs/coder-service --self-contained
tp: # terraform plan
	cd infrastructure/iac/terraform/environments/staging && terraform plan
ta: # terraform apply
	cd infrastructure/iac/terraform/environments/staging && terraform apply
td: # terraform destroy
	cd infrastructure/iac/terraform/environments/staging && terraform destroy
dcu: # docker-compose up : webapi
	cd docker-compose/ && docker-compose up -d
dcd: # docker-compose down : webapi 
	cd docker-compose/ && docker-compose down
fds: # force rededeploy aws ecs service
	aws ecs update-service --force-new-deployment --service coder-service --cluster genocs
gw: # git docker workflow to push docker image to the repository based on the main branch
	@echo triggering github workflow to push docker image to container
	@echo ensure that you have the gh-cli installed and authenticated.
	gh workflow run dockerhub-publish -f push_to_docker=true