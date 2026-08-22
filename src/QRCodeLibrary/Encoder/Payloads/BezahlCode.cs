using System.Text.RegularExpressions;
using Genocs.QRCodeLibrary.Encoder.Helpers;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

/// <summary>
/// Generates a BezahlCode payload for QR code encoding.
/// BezahlCode is a German standard for encoding payment information in QR codes,
/// allowing users to initiate payments easily by scanning the code with their banking app.
/// BezahlCode is obsolete and has been replaced by Girocode, which is a more widely adopted standard for SEPA payments in Europe.
/// </summary>
internal class BezahlCode : Payload
{
    private readonly string name, iban, bic, account, bnc, sepaReference, reason, creditorId, mandateId, periodicTimeunit;
    private readonly decimal amount;
    private readonly int postingKey, periodicTimeunitRotation;
    private readonly Currency currency;
    private readonly BezahlCode.AuthorityType authority;
    private readonly DateTime executionDate, dateOfSignature, periodicFirstExecutionDate, periodicLastExecutionDate;

    /// <summary>
    /// Constructor for contact data.
    /// </summary>
    /// <param name="authority">Type of the bank transfer</param>
    /// <param name="name">Name of the receiver (Empfänger)</param>
    /// <param name="account">Bank account (Kontonummer)</param>
    /// <param name="bnc">Bank institute (Bankleitzahl)</param>
    /// <param name="iban">IBAN</param>
    /// <param name="bic">BIC</param>
    /// <param name="reason">Reason (Verwendungszweck)</param>
    public BezahlCode(BezahlCode.AuthorityType authority, string name, string account = "", string bnc = "", string iban = "", string bic = "", string reason = "")
        : this(authority, name, account, bnc, iban, bic, 0, string.Empty, 0, null, null, string.Empty, string.Empty, null, reason, 0, string.Empty, Currency.EUR, null, 1)
    {
    }

    /// <summary>
    /// Constructor for non-SEPA payments.
    /// </summary>
    /// <param name="authority">Type of the bank transfer.</param>
    /// <param name="name">Name of the receiver (Empfänger).</param>
    /// <param name="account">Bank account (Kontonummer).</param>
    /// <param name="bnc">Bank institute (Bankleitzahl).</param>
    /// <param name="amount">Amount (Betrag)</param>
    /// <param name="periodicTimeunit">Unit of intervall for payment ('M' = monthly, 'W' = weekly).</param>
    /// <param name="periodicTimeunitRotation">Intervall for payment. This value is combined with 'periodicTimeunit'.</param>
    /// <param name="periodicFirstExecutionDate">Date of first periodic execution.</param>
    /// <param name="periodicLastExecutionDate">Date of last periodic execution.</param>
    /// <param name="reason">Reason (Verwendungszweck).</param>
    /// <param name="postingKey">Transfer Key (Textschlüssel, z.B. Spendenzahlung = 69).</param>
    /// <param name="currency">Currency (Währung).</param>
    /// <param name="executionDate">Execution date (Ausführungsdatum).</param>
    public BezahlCode(BezahlCode.AuthorityType authority, string name, string account, string bnc, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string reason = "", int postingKey = 0, Currency currency = Currency.EUR, DateTime? executionDate = null)
        : this(authority, name, account, bnc, string.Empty, string.Empty, amount, periodicTimeunit, periodicTimeunitRotation, periodicFirstExecutionDate, periodicLastExecutionDate, string.Empty, string.Empty, null, reason, postingKey, string.Empty, currency, executionDate, 2)
    {
    }

    /// <summary>
    /// Constructor for SEPA payments
    /// </summary>
    /// <param name="authority">Type of the bank transfer</param>
    /// <param name="name">Name of the receiver (Empfänger)</param>
    /// <param name="iban">IBAN</param>
    /// <param name="bic">BIC</param>
    /// <param name="amount">Amount (Betrag)</param>
    /// <param name="periodicTimeunit">Unit of intervall for payment ('M' = monthly, 'W' = weekly)</param>
    /// <param name="periodicTimeunitRotation">Intervall for payment. This value is combined with 'periodicTimeunit'</param>
    /// <param name="periodicFirstExecutionDate">Date of first periodic execution</param>
    /// <param name="periodicLastExecutionDate">Date of last periodic execution</param>
    /// <param name="creditorId">Creditor id (Gläubiger ID)</param>
    /// <param name="mandateId">Manadate id (Mandatsreferenz)</param>
    /// <param name="dateOfSignature">Signature date (Erteilungsdatum des Mandats)</param>
    /// <param name="reason">Reason (Verwendungszweck)</param>
    /// <param name="postingKey">Transfer Key (Textschlüssel, z.B. Spendenzahlung = 69)</param>
    /// <param name="sepaReference">SEPA reference (SEPA-Referenz)</param>
    /// <param name="currency">Currency (Währung)</param>
    /// <param name="executionDate">Execution date (Ausführungsdatum)</param>
    public BezahlCode(BezahlCode.AuthorityType authority, string name, string iban, string bic, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string creditorId = "", string mandateId = "", DateTime? dateOfSignature = null, string reason = "", string sepaReference = "", Currency currency = Currency.EUR, DateTime? executionDate = null)
        : this(authority, name, string.Empty, string.Empty, iban, bic, amount, periodicTimeunit, periodicTimeunitRotation, periodicFirstExecutionDate, periodicLastExecutionDate, creditorId, mandateId, dateOfSignature, reason, 0, sepaReference, currency, executionDate, 3)
    {
    }

    /// <summary>
    /// Generic constructor. Please use specific (non-SEPA or SEPA) constructor.
    /// </summary>
    /// <param name="authority">Type of the bank transfer.</param>
    /// <param name="name">Name of the receiver (Empfänger).</param>
    /// <param name="account">Bank account (Kontonummer).</param>
    /// <param name="bnc">Bank institute (Bankleitzahl).</param>
    /// <param name="iban">IBAN</param>
    /// <param name="bic">BIC</param>
    /// <param name="amount">Amount (Betrag)</param>
    /// <param name="periodicTimeunit">Unit of intervall for payment ('M' = monthly, 'W' = weekly).</param>
    /// <param name="periodicTimeunitRotation">Intervall for payment. This value is combined with 'periodicTimeunit'.</param>
    /// <param name="periodicFirstExecutionDate">Date of first periodic execution</param>
    /// <param name="periodicLastExecutionDate">Date of last periodic execution</param>
    /// <param name="creditorId">Creditor id (Gläubiger ID)</param>
    /// <param name="mandateId">Manadate id (Mandatsreferenz)</param>
    /// <param name="dateOfSignature">Signature date (Erteilungsdatum des Mandats)</param>
    /// <param name="reason">Reason (Verwendungszweck)</param>
    /// <param name="postingKey">Transfer Key (Textschlüssel, z.B. Spendenzahlung = 69)</param>
    /// <param name="sepaReference">SEPA reference (SEPA-Referenz)</param>
    /// <param name="currency">Currency (Währung)</param>
    /// <param name="executionDate">Execution date (Ausführungsdatum)</param>
    /// <param name="internalMode">Only used for internal state handdling</param>
    public BezahlCode(
        AuthorityType authority,
        string name,
        string account,
        string bnc,
        string iban,
        string bic,
        decimal amount,
        string periodicTimeunit = "",
        int periodicTimeunitRotation = 0,
        DateTime? periodicFirstExecutionDate = null,
        DateTime? periodicLastExecutionDate = null,
        string creditorId = "",
        string mandateId = "",
        DateTime? dateOfSignature = null,
        string reason = "",
        int postingKey = 0,
        string sepaReference = "",
        Currency currency = Currency.EUR,
        DateTime? executionDate = null,
        int internalMode = 0)
    {
        // Loaded via "contact-constructor"
        if (internalMode == 1)
        {
            if (authority != AuthorityType.contact && authority != AuthorityType.contact_v2)
                throw new BezahlCodeException("The constructor without an amount may only ne used with authority types 'contact' and 'contact_v2'.");

            if (authority == AuthorityType.contact && (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(bnc)))
                throw new BezahlCodeException("When using authority type 'contact' the parameters 'account' and 'bnc' must be set.");

            if (authority != AuthorityType.contact_v2)
            {
                var oldFilled = !string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(bnc);
                var newFilled = !string.IsNullOrEmpty(iban) && !string.IsNullOrEmpty(bic);
                if (!oldFilled && !newFilled || oldFilled && newFilled)
                    throw new BezahlCodeException("When using authority type 'contact_v2' either the parameters 'account' and 'bnc' or the parameters 'iban' and 'bic' must be set. Leave the other parameter pair empty.");
            }
        }
        else if (internalMode == 2)
        {
            if (authority != AuthorityType.periodicsinglepayment && authority != AuthorityType.singledirectdebit && authority != AuthorityType.SinglePayment)
                throw new BezahlCodeException("The constructor with 'account' and 'bnc' may only be used with 'non SEPA' authority types. Either choose another authority type or switch constructor.");
            if (authority == AuthorityType.periodicsinglepayment && (string.IsNullOrEmpty(periodicTimeunit) || periodicTimeunitRotation == 0))
                throw new BezahlCodeException("When using 'periodicsinglepayment' as authority type, the parameters 'periodicTimeunit' and 'periodicTimeunitRotation' must be set.");

        }
        else if (internalMode == 3)
        {
            if (authority != AuthorityType.periodicsinglepaymentsepa && authority != AuthorityType.singledirectdebitsepa && authority != AuthorityType.SinglePaymentSepa)
                throw new BezahlCodeException("The constructor with 'iban' and 'bic' may only be used with 'SEPA' authority types. Either choose another authority type or switch constructor.");
            if (authority == AuthorityType.periodicsinglepaymentsepa && (string.IsNullOrEmpty(periodicTimeunit) || periodicTimeunitRotation == 0))
                throw new BezahlCodeException("When using 'periodicsinglepaymentsepa' as authority type, the parameters 'periodicTimeunit' and 'periodicTimeunitRotation' must be set.");
        }

        this.authority = authority;

        if (name.Length > 70)
            throw new BezahlCodeException("(Payee-)Name must be shorter than 71 chars.");
        this.name = name;

        if (reason.Length > 27)
            throw new BezahlCodeException("Reasons texts have to be shorter than 28 chars.");
        this.reason = reason;

        var oldWayFilled = !string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(bnc);
        var newWayFilled = !string.IsNullOrEmpty(iban) && !string.IsNullOrEmpty(bic);

        // Non-SEPA payment types
        if (authority == AuthorityType.periodicsinglepayment
            || authority == AuthorityType.singledirectdebit
            || authority == AuthorityType.SinglePayment
            || authority == AuthorityType.contact
            || authority == AuthorityType.contact_v2 && oldWayFilled)
        {
            if (!Regex.IsMatch(account.Replace(" ", ""), @"^[0-9]{1,9}$"))
                throw new BezahlCodeException("The account entered isn't valid.");
            this.account = account.Replace(" ", "").ToUpper();
            if (!Regex.IsMatch(bnc.Replace(" ", ""), @"^[0-9]{1,9}$"))
                throw new BezahlCodeException("The bnc entered isn't valid.");
            this.bnc = bnc.Replace(" ", "").ToUpper();

            if (authority != AuthorityType.contact && authority != AuthorityType.contact_v2)
            {
                if (postingKey < 0 || postingKey >= 100)
                    throw new BezahlCodeException("PostingKey must be within 0 and 99.");
                this.postingKey = postingKey;
            }
        }

        // SEPA payment types
        if (authority == AuthorityType.periodicsinglepaymentsepa
            || authority == AuthorityType.singledirectdebitsepa
            || authority == AuthorityType.SinglePaymentSepa
            || authority == AuthorityType.contact_v2 && newWayFilled)
        {
            if (!StringHelper.IsValidIban(iban))
                throw new BezahlCodeException("The IBAN entered isn't valid.");

            if (!StringHelper.IsValidBic(bic))
                throw new BezahlCodeException("The BIC entered isn't valid.");

            this.iban = iban.Replace(" ", string.Empty).ToUpper();
            this.bic = bic.Replace(" ", string.Empty).ToUpper();

            if (authority != BezahlCode.AuthorityType.contact_v2)
            {
                if (sepaReference.Length > 35)
                    throw new BezahlCodeException("SEPA reference texts have to be shorter than 36 chars.");
                this.sepaReference = sepaReference;

                if (!string.IsNullOrEmpty(creditorId) && !Regex.IsMatch(creditorId.Replace(" ", ""), @"^[a-zA-Z]{2,2}[0-9]{2,2}([A-Za-z0-9]|[\+|\?|/|\-|:|\(|\)|\.|,|']){3,3}([A-Za-z0-9]|[\+|\?|/|\-|:|\(|\)|\.|,|']){1,28}$"))
                    throw new BezahlCodeException("The creditorId entered isn't valid.");
                this.creditorId = creditorId;
                if (!string.IsNullOrEmpty(mandateId) && !Regex.IsMatch(mandateId.Replace(" ", ""), @"^([A-Za-z0-9]|[\+|\?|/|\-|:|\(|\)|\.|,|']){1,35}$"))
                    throw new BezahlCodeException("The mandateId entered isn't valid.");
                this.mandateId = mandateId;
                if (dateOfSignature != null)
                    this.dateOfSignature = (DateTime)dateOfSignature;
            }
        }

        // Checks for all payment types
        if (authority != AuthorityType.contact && authority != AuthorityType.contact_v2)
        {
            if (amount.ToString().Replace(",", ".").Contains(".") && amount.ToString().Replace(",", ".").Split('.')[1].TrimEnd('0').Length > 2)
                throw new BezahlCodeException("Amount must have less than 3 digits after decimal point.");
            if (amount < 0.01m || amount > 999999999.99m)
                throw new BezahlCodeException("Amount has to at least 0.01 and must be smaller or equal to 999999999.99.");
            this.amount = amount;

            this.currency = currency;

            if (executionDate == null)
            {
                this.executionDate = DateTime.Now;
            }
            else
            {
                if (DateTime.Today.Ticks > executionDate.Value.Ticks)
                {
                    throw new BezahlCodeException("Execution date must be today or in future.");
                }

                this.executionDate = (DateTime)executionDate;
            }

            if (authority == AuthorityType.periodicsinglepayment || authority == BezahlCode.AuthorityType.periodicsinglepaymentsepa)
            {
                if (periodicTimeunit.ToUpper() != "M" && periodicTimeunit.ToUpper() != "W")
                    throw new BezahlCodeException("The periodicTimeunit must be either 'M' (monthly) or 'W' (weekly).");
                this.periodicTimeunit = periodicTimeunit;
                if (periodicTimeunitRotation < 1 || periodicTimeunitRotation > 52)
                    throw new BezahlCodeException("The periodicTimeunitRotation must be 1 or greater. (It means repeat the payment every 'periodicTimeunitRotation' weeks/months.");
                this.periodicTimeunitRotation = periodicTimeunitRotation;
                if (periodicFirstExecutionDate != null)
                    this.periodicFirstExecutionDate = (DateTime)periodicFirstExecutionDate;
                if (periodicLastExecutionDate != null)
                    this.periodicLastExecutionDate = (DateTime)periodicLastExecutionDate;
            }
        }
    }

    public override string ToString()
    {
        var bezahlCodePayload = $"bank://{authority.ToString().ToLower()}?";

        bezahlCodePayload += $"name={Uri.EscapeDataString(name)}&";

        if (authority != BezahlCode.AuthorityType.contact && authority != BezahlCode.AuthorityType.contact_v2)
        {
            // Handle what is same for all payments

            if (authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.singledirectdebit || authority == AuthorityType.SinglePayment)
            {
                bezahlCodePayload += $"account={account}&";
                bezahlCodePayload += $"bnc={bnc}&";
                if (postingKey > 0)
                    bezahlCodePayload += $"postingkey={postingKey}&";
            }
            else
            {
                bezahlCodePayload += $"iban={iban}&";
                bezahlCodePayload += $"bic={bic}&";

                if (!string.IsNullOrEmpty(sepaReference))
                    bezahlCodePayload += $"separeference={Uri.EscapeDataString(sepaReference)}&";

                if (authority == BezahlCode.AuthorityType.singledirectdebitsepa)
                {
                    if (!string.IsNullOrEmpty(creditorId))
                        bezahlCodePayload += $"creditorid={Uri.EscapeDataString(creditorId)}&";
                    if (!string.IsNullOrEmpty(mandateId))
                        bezahlCodePayload += $"mandateid={Uri.EscapeDataString(mandateId)}&";
                    if (dateOfSignature != null)
                        bezahlCodePayload += $"dateofsignature={dateOfSignature.ToString("ddMMyyyy")}&";
                }
            }

            bezahlCodePayload += $"amount={amount:0.00}&".Replace(".", ",");

            if (!string.IsNullOrEmpty(reason))
                bezahlCodePayload += $"reason={Uri.EscapeDataString(reason)}&";
            bezahlCodePayload += $"currency={currency}&";
            bezahlCodePayload += $"executiondate={executionDate.ToString("ddMMyyyy")}&";

            if (authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.periodicsinglepaymentsepa)
            {
                bezahlCodePayload += $"periodictimeunit={periodicTimeunit}&";
                bezahlCodePayload += $"periodictimeunitrotation={periodicTimeunitRotation}&";
                if (periodicFirstExecutionDate != null)
                    bezahlCodePayload += $"periodicfirstexecutiondate={periodicFirstExecutionDate.ToString("ddMMyyyy")}&";
                if (periodicLastExecutionDate != null)
                    bezahlCodePayload += $"periodiclastexecutiondate={periodicLastExecutionDate.ToString("ddMMyyyy")}&";
            }
        }
        else
        {
            // Handle what is same for all contacts
            if (authority == BezahlCode.AuthorityType.contact)
            {
                bezahlCodePayload += $"account={account}&";
                bezahlCodePayload += $"bnc={bnc}&";
            }
            else if (authority == BezahlCode.AuthorityType.contact_v2)
            {
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(bnc))
                {
                    bezahlCodePayload += $"account={account}&";
                    bezahlCodePayload += $"bnc={bnc}&";
                }
                else
                {
                    bezahlCodePayload += $"iban={iban}&";
                    bezahlCodePayload += $"bic={bic}&";
                }
            }

            if (!string.IsNullOrEmpty(reason))
                bezahlCodePayload += $"reason={Uri.EscapeDataString(reason)}&";
        }

        return bezahlCodePayload.Trim('&');
    }

    /// <summary>
    /// Operation modes of the BezahlCode.
    /// </summary>
    public enum AuthorityType
    {
        /// <summary>
        /// Single payment (Überweisung).
        /// </summary>
        [Obsolete]
        SinglePayment,

        /// <summary>
        /// Single SEPA payment (SEPA-Überweisung).
        /// </summary>
        SinglePaymentSepa,

        /// <summary>
        /// Single debit (Lastschrift).
        /// </summary>
        [Obsolete]
        singledirectdebit,

        /// <summary>
        /// Single SEPA debit (SEPA-Lastschrift).
        /// </summary>
        singledirectdebitsepa,

        /// <summary>
        /// Periodic payment (Dauerauftrag).
        /// </summary>
        [Obsolete]
        periodicsinglepayment,

        /// <summary>
        /// Periodic SEPA payment (SEPA-Dauerauftrag).
        /// </summary>
        periodicsinglepaymentsepa,

        /// <summary>
        /// Contact data.
        /// </summary>
        contact,

        /// <summary>
        /// Contact data V2.
        /// </summary>
        contact_v2
    }

    public class BezahlCodeException : Exception
    {
        public BezahlCodeException()
        {
        }

        public BezahlCodeException(string message)
            : base(message)
        {
        }

        public BezahlCodeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
