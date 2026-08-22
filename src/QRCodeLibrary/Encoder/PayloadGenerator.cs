using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Genocs.QRCodeLibrary.Encoder.Helpers;
using Genocs.QRCodeLibrary.Encoder.Payloads;

namespace Genocs.QRCodeLibrary.Encoder;

public static partial class PayloadGenerator
{

    internal class SMS : Payload
    {
        private readonly string number, subject;
        private readonly SMSEncoding encoding;

        /// <summary>
        /// Creates a SMS payload without text.
        /// </summary>
        /// <param name="number">Receiver phone number.</param>
        /// <param name="encoding">Encoding type.</param>
        public SMS(string number, SMSEncoding encoding = SMSEncoding.SMS)
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        /// <summary>
        /// Creates a SMS payload with text (subject)
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="subject">Text of the SMS</param>
        /// <param name="encoding">Encoding type</param>
        public SMS(string number, string subject, SMSEncoding encoding = SMSEncoding.SMS)
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        public override string ToString()
        {
            return encoding switch
            {
                SMSEncoding.SMS => $"sms:{number}?body={Uri.EscapeDataString(subject)}",
                SMSEncoding.SMS_iOS => $"sms:{number};body={Uri.EscapeDataString(subject)}",
                SMSEncoding.SMSTO => $"SMSTO:{number}:{subject}",
                _ => "sms:",
            };
        }

        public enum SMSEncoding
        {
            SMS,
            SMSTO,
            SMS_iOS
        }
    }

    internal class MMS : Payload
    {
        private readonly string number, subject;
        private readonly MMSEncoding encoding;

        /// <summary>
        /// Creates a MMS payload without text.
        /// </summary>
        /// <param name="number">Receiver phone number.</param>
        /// <param name="encoding">Encoding type.</param>
        public MMS(string number, MMSEncoding encoding = MMSEncoding.MMS)
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        /// <summary>
        /// Creates a MMS payload with text (subject).
        /// </summary>
        /// <param name="number">Receiver phone number.</param>
        /// <param name="subject">Text of the MMS.</param>
        /// <param name="encoding">Encoding type.</param>
        public MMS(string number, string subject, MMSEncoding encoding = MMSEncoding.MMS)
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        public override string ToString()
        {
            return encoding switch
            {
                MMSEncoding.MMSTO => $"mmsto:{number}?subject={Uri.EscapeDataString(subject)}",
                MMSEncoding.MMS => $"mms:{number}?body={Uri.EscapeDataString(subject)}",
                _ => "mms:",
            };
        }

        public enum MMSEncoding
        {
            MMS,
            MMSTO
        }
    }

    internal class Geolocation : Payload
    {
        private readonly string latitude, longitude;
        private readonly GeolocationEncoding encoding;

        /// <summary>
        /// Generates a geo location payload. Supports raw location (GEO encoding) or Google Maps link (GoogleMaps encoding)
        /// </summary>
        /// <param name="latitude">Latitude with . as splitter</param>
        /// <param name="longitude">Longitude with . as splitter</param>
        /// <param name="encoding">Encoding type - GEO or GoogleMaps</param>
        public Geolocation(string latitude, string longitude, GeolocationEncoding encoding = GeolocationEncoding.GEO)
        {
            this.latitude = latitude.Replace(",", ".");
            this.longitude = longitude.Replace(",", ".");
            this.encoding = encoding;
        }

        public override string ToString()
        {
            return encoding switch
            {
                GeolocationEncoding.GEO => $"geo:{latitude},{longitude}",
                GeolocationEncoding.GoogleMaps => $"http://maps.google.com/maps?q={latitude},{longitude}",
                _ => "geo:",
            };
        }

        public enum GeolocationEncoding
        {
            GEO,
            GoogleMaps
        }
    }

    internal class SkypeCall : Payload
    {
        private readonly string _username;

        /// <summary>
        /// Generates a Skype call payload.
        /// </summary>
        /// <param name="skypeUsername">Skype username which will be called.</param>
        public SkypeCall(string skypeUsername)
        {
            _username = skypeUsername;
        }

        public override string ToString()
            => $"skype:{_username}?call";
    }

    internal class Url : Payload
    {
        private readonly string _url;

        /// <summary>
        /// Generates a link. If not given, http/https protocol will be added.
        /// </summary>
        /// <param name="url">Link url target.</param>
        public Url(string url)
        {
            _url = url;
        }

        public override string ToString()
        {
            return !_url.StartsWith("http") ? "http://" + _url : _url;
        }
    }

    internal class WhatsAppMessage : Payload
    {
        private readonly string _number;
        private readonly string _message;

        /// <summary>
        /// Let's you compose a WhatApp message and send it the receiver number.
        /// </summary>
        /// <param name="number">Receiver phone number.</param>
        /// <param name="message">The message.</param>
        public WhatsAppMessage(string number, string message)
        {
            _number = number;
            _message = Uri.EscapeDataString(message);
        }

        /// <summary>
        /// Let's you compose a WhatApp message. When scanned the user is asked to choose a contact who will receive the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public WhatsAppMessage(string message)
        {
            _number = string.Empty;
            _message = Uri.EscapeDataString(message);
        }

        public override string ToString()
        {
            return $"whatsapp://send?phone={_number}&text={_message}";
        }
    }

    internal class SwissQrCode : Payload
    {
        //Keep in mind, that the ECC level has to be set to "M" when generating a SwissQrCode!
        //SwissQrCode specification: 
        //    - (de) https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-de.pdf
        //    - (en) https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-en.pdf
        //Changes between version 1.0 and 2.0: https://www.paymentstandards.ch/dam/downloads/change-documentation-qrr-de.pdf

        private readonly string br = "\r\n";
        private readonly string alternativeProcedure1, alternativeProcedure2;
        private readonly Iban iban;
        private readonly decimal? amount;
        private readonly Contact creditor, ultimateCreditor, debitor;
        private readonly Currency currency;
        private readonly DateTime? requestedDateOfPayment;
        private readonly Reference reference;
        private readonly AdditionalInformation additionalInformation;

        /// <summary>
        /// Generates the payload for a SwissQrCode v2.0. (Don't forget to use ECC-Level=M, EncodingMode=UTF-8 and to set the Swiss flag icon to the final QR code.)
        /// </summary>
        /// <param name="iban">IBAN object</param>
        /// <param name="currency">Currency (either EUR or CHF)</param>
        /// <param name="creditor">Creditor (payee) information</param>
        /// <param name="reference">Reference information</param>
        /// <param name="additionalInformation">Additional information (unstructured message and bill information)</param>
        /// <param name="debitor">Debitor (payer) information</param>
        /// <param name="amount">Amount</param>
        /// <param name="requestedDateOfPayment">Requested date of debitor's payment</param>
        /// <param name="ultimateCreditor">Ultimate creditor information (use only in consultation with your bank - for future use only!)</param>
        /// <param name="alternativeProcedure1">Optional command for alternative processing mode - line 1</param>
        /// <param name="alternativeProcedure2">Optional command for alternative processing mode - line 2</param>
        public SwissQrCode(Iban iban, Currency currency, Contact creditor, Reference reference, AdditionalInformation? additionalInformation = null, Contact? debitor = null, decimal? amount = null, DateTime? requestedDateOfPayment = null, Contact ultimateCreditor = null, string alternativeProcedure1 = null, string alternativeProcedure2 = null)
        {
            this.iban = iban;

            this.creditor = creditor;
            this.ultimateCreditor = ultimateCreditor;

            this.additionalInformation = additionalInformation != null ? additionalInformation : new AdditionalInformation();

            if (amount != null && amount.ToString().Length > 12)
                throw new SwissQrCodeException("Amount (including decimals) must be shorter than 13 places.");
            this.amount = amount;

            this.currency = currency;
            this.requestedDateOfPayment = requestedDateOfPayment;
            this.debitor = debitor;

            if (iban.IsQrIban && reference.RefType != Reference.ReferenceType.QRR)
                throw new SwissQrCodeException("If QR-IBAN is used, you have to choose \"QRR\" as reference type!");
            if (!iban.IsQrIban && reference.RefType == Reference.ReferenceType.QRR)
                throw new SwissQrCodeException("If non QR-IBAN is used, you have to choose either \"SCOR\" or \"NON\" as reference type!");
            this.reference = reference;

            if (alternativeProcedure1 != null && alternativeProcedure1.Length > 100)
                throw new SwissQrCodeException("Alternative procedure information block 1 must be shorter than 101 chars.");
            this.alternativeProcedure1 = alternativeProcedure1;
            if (alternativeProcedure2 != null && alternativeProcedure2.Length > 100)
                throw new SwissQrCodeException("Alternative procedure information block 2 must be shorter than 101 chars.");
            this.alternativeProcedure2 = alternativeProcedure2;
        }

        public class AdditionalInformation
        {
            private readonly string unstructuredMessage, billInformation, trailer;

            /// <summary>
            /// Creates an additional information object. Both parameters are optional and must be shorter than 141 chars in combination.
            /// </summary>
            /// <param name="unstructuredMessage">Unstructured text message</param>
            /// <param name="billInformation">Bill information</param>
            public AdditionalInformation(string? unstructuredMessage = null, string? billInformation = null)
            {
                if ((unstructuredMessage != null ? unstructuredMessage.Length : 0) + (billInformation != null ? billInformation.Length : 0) > 140)
                    throw new SwissQrCodeAdditionalInformationException("Unstructured message and bill information must be shorter than 141 chars in total/combined.");
                this.unstructuredMessage = unstructuredMessage;
                this.billInformation = billInformation;
                trailer = "EPD";
            }

            public string UnstructureMessage
            {
                get { return !string.IsNullOrEmpty(unstructuredMessage) ? unstructuredMessage.Replace("\n", "") : null; }
            }

            public string BillInformation
            {
                get { return !string.IsNullOrEmpty(billInformation) ? billInformation.Replace("\n", "") : null; }
            }

            public string Trailer
            {
                get { return trailer; }
            }

            public class SwissQrCodeAdditionalInformationException : Exception
            {
                public SwissQrCodeAdditionalInformationException()
                {
                }

                public SwissQrCodeAdditionalInformationException(string message)
                    : base(message)
                {
                }

                public SwissQrCodeAdditionalInformationException(string message, Exception inner)
                    : base(message, inner)
                {
                }
            }
        }

        public class Reference
        {
            private readonly string? _reference;
            private readonly ReferenceTextType? _referenceTextType;

            /// <summary>
            /// Creates a reference object which must be passed to the SwissQrCode instance.
            /// </summary>
            /// <param name="referenceType">Type of the reference (QRR, SCOR or NON).</param>
            /// <param name="reference">Reference text</param>
            /// <param name="referenceTextType">Type of the reference text (QR-reference or Creditor Reference).</param>
            public Reference(ReferenceType referenceType, string? reference = null, ReferenceTextType? referenceTextType = null)
            {
                RefType = referenceType;
                _referenceTextType = referenceTextType;

                if (referenceType == ReferenceType.NON && reference != null)
                    throw new SwissQrCodeReferenceException("Reference is only allowed when referenceType not equals \"NON\"");
                if (referenceType != ReferenceType.NON && reference != null && referenceTextType == null)
                    throw new SwissQrCodeReferenceException("You have to set an ReferenceTextType when using the reference text.");
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && reference.Length > 27)
                    throw new SwissQrCodeReferenceException("QR-references have to be shorter than 28 chars.");
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && !Regex.IsMatch(reference, @"^[0-9]+$"))
                    throw new SwissQrCodeReferenceException("QR-reference must exist out of digits only.");
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && !ChecksumMod10(reference))
                    throw new SwissQrCodeReferenceException("QR-references is invalid. Checksum error.");
                if (referenceTextType == ReferenceTextType.CreditorReferenceIso11649 && reference != null && reference.Length > 25)
                    throw new SwissQrCodeReferenceException("Creditor references (ISO 11649) have to be shorter than 26 chars.");

                this._reference = reference;
            }

            public ReferenceType? RefType { get; }

            public string? ReferenceText
            {
                get { return !string.IsNullOrEmpty(_reference) ? _reference.Replace("\n", string.Empty) : null; }
            }

            /// <summary>
            /// Reference type. When using a QR-IBAN you have to use either "QRR" or "SCOR"
            /// </summary>
            public enum ReferenceType
            {
                QRR,
                SCOR,
                NON
            }

            public enum ReferenceTextType
            {
                QrReference,
                CreditorReferenceIso11649
            }

            public class SwissQrCodeReferenceException : Exception
            {
                public SwissQrCodeReferenceException()
                {
                }

                public SwissQrCodeReferenceException(string message)
                    : base(message)
                {
                }

                public SwissQrCodeReferenceException(string message, Exception inner)
                    : base(message, inner)
                {
                }
            }
        }

        public class Iban
        {
            private string _iban;
            private IbanType _ibanType;

            /// <summary>
            /// IBAN object with type information.
            /// </summary>
            /// <param name="iban">The IBAN to validate.</param>
            /// <param name="ibanType">Type of IBAN (normal or QR-IBAN).</param>
            public Iban(string iban, IbanType ibanType)
            {
                if (ibanType == IbanType.Iban && !StringHelper.IsValidIban(iban))
                    throw new SwissQrCodeIbanException("The IBAN entered isn't valid.");
                if (ibanType == IbanType.QrIban && !StringHelper.IsValidQRIban(iban))
                    throw new SwissQrCodeIbanException("The QR-IBAN entered isn't valid.");
                if (!iban.StartsWith("CH") && !iban.StartsWith("LI"))
                    throw new SwissQrCodeIbanException("The IBAN must start with \"CH\" or \"LI\".");
                _iban = iban;
                _ibanType = ibanType;
            }

            public bool IsQrIban
            {
                get { return _ibanType == IbanType.QrIban; }
            }

            public override string ToString()
            {
                return _iban.Replace("-", "").Replace("\n", "").Replace(" ", "");
            }

            public enum IbanType
            {
                Iban,
                QrIban
            }

            public class SwissQrCodeIbanException : Exception
            {
                public SwissQrCodeIbanException()
                {
                }

                public SwissQrCodeIbanException(string message)
                    : base(message)
                {
                }

                public SwissQrCodeIbanException(string message, Exception inner)
                    : base(message, inner)
                {
                }
            }
        }

        public class Contact
        {
            private static readonly HashSet<string> twoLetterCodes = ValidTwoLetterCodes();
            private string br = "\r\n";
            private string name, streetOrAddressline1, houseNumberOrAddressline2, zipCode, city, country;
            private AddressType adrType;

            /// <summary>
            /// Contact type. Can be used for payee, ultimate payee, etc. with address in structured mode (S).
            /// </summary>
            /// <param name="name">Last name or company (optional first name)</param>
            /// <param name="zipCode">Zip-/Postcode</param>
            /// <param name="city">City name</param>
            /// <param name="country">Two-letter country code as defined in ISO 3166-1</param>
            /// <param name="street">Streetname without house number</param>
            /// <param name="houseNumber">House number</param>
            [Obsolete("This constructor is deprecated. Use WithStructuredAddress instead.")]
            public Contact(string name, string zipCode, string city, string country, string street = null, string houseNumber = null) : this(name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress)
            {
            }


            /// <summary>
            /// Contact type. Can be used for payee, ultimate payee, etc. with address in combined mode (K).
            /// </summary>
            /// <param name="name">Last name or company (optional first name)</param>
            /// <param name="country">Two-letter country code as defined in ISO 3166-1</param>
            /// <param name="addressLine1">Adress line 1</param>
            /// <param name="addressLine2">Adress line 2</param>
            [Obsolete("This constructor is deprecated. Use WithCombinedAddress instead.")]
            public Contact(string name, string country, string addressLine1, string addressLine2) : this(name, null, null, country, addressLine1, addressLine2, AddressType.CombinedAddress)
            {
            }

            public static Contact WithStructuredAddress(string name, string zipCode, string city, string country, string street = null, string houseNumber = null)
            {
                return new Contact(name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress);
            }

            public static Contact WithCombinedAddress(string name, string country, string addressLine1, string addressLine2)
            {
                return new Contact(name, null, null, country, addressLine1, addressLine2, AddressType.CombinedAddress);
            }

            private Contact(string name, string zipCode, string city, string country, string streetOrAddressline1, string houseNumberOrAddressline2, AddressType addressType)
            {
                // Pattern extracted from https://qr-validation.iso-payments.ch as explained in https://github.com/codebude/QRCoder/issues/97
                var charsetPattern = @"^([a-zA-Z0-9\.,;:'\ \+\-/\(\)?\*\[\]\{\}\\`´~ ]|[!""#%&<>÷=@_$£]|[àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ])*$";

                adrType = addressType;

                if (string.IsNullOrEmpty(name))
                    throw new SwissQrCodeContactException("Name must not be empty.");
                if (name.Length > 70)
                    throw new SwissQrCodeContactException("Name must be shorter than 71 chars.");
                if (!Regex.IsMatch(name, charsetPattern))
                    throw new SwissQrCodeContactException($"Name must match the following pattern as defined in pain.001: {charsetPattern}");
                this.name = name;

                if (AddressType.StructuredAddress == adrType)
                {
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && streetOrAddressline1.Length > 70)
                        throw new SwissQrCodeContactException("Street must be shorter than 71 chars.");
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && !Regex.IsMatch(streetOrAddressline1, charsetPattern))
                        throw new SwissQrCodeContactException($"Street must match the following pattern as defined in pain.001: {charsetPattern}");
                    this.streetOrAddressline1 = streetOrAddressline1;

                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && houseNumberOrAddressline2.Length > 16)
                        throw new SwissQrCodeContactException("House number must be shorter than 17 chars.");
                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && streetOrAddressline1.Length > 70)
                        throw new SwissQrCodeContactException("Address line 1 must be shorter than 71 chars.");
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && !Regex.IsMatch(streetOrAddressline1, charsetPattern))
                        throw new SwissQrCodeContactException($"Address line 1 must match the following pattern as defined in pain.001: {charsetPattern}");
                    this.streetOrAddressline1 = streetOrAddressline1;

                    if (string.IsNullOrEmpty(houseNumberOrAddressline2))
                        throw new SwissQrCodeContactException("Address line 2 must be provided for combined addresses (address line-based addresses).");
                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && houseNumberOrAddressline2.Length > 70)
                        throw new SwissQrCodeContactException("Address line 2 must be shorter than 71 chars.");
                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && !Regex.IsMatch(houseNumberOrAddressline2, charsetPattern))
                        throw new SwissQrCodeContactException($"Address line 2 must match the following pattern as defined in pain.001: {charsetPattern}");
                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }

                if (AddressType.StructuredAddress == adrType)
                {
                    if (string.IsNullOrEmpty(zipCode))
                        throw new SwissQrCodeContactException("Zip code must not be empty.");
                    if (zipCode.Length > 16)
                        throw new SwissQrCodeContactException("Zip code must be shorter than 17 chars.");
                    if (!Regex.IsMatch(zipCode, charsetPattern))
                        throw new SwissQrCodeContactException($"Zip code must match the following pattern as defined in pain.001: {charsetPattern}");
                    this.zipCode = zipCode;

                    if (string.IsNullOrEmpty(city))
                        throw new SwissQrCodeContactException("City must not be empty.");
                    if (city.Length > 35)
                        throw new SwissQrCodeContactException("City name must be shorter than 36 chars.");
                    if (!Regex.IsMatch(city, charsetPattern))
                        throw new SwissQrCodeContactException($"City name must match the following pattern as defined in pain.001: {charsetPattern}");
                    this.city = city;
                }
                else
                {
                    this.zipCode = this.city = string.Empty;
                }

                if (!IsValidTwoLetterCode(country))
                    throw new SwissQrCodeContactException("Country must be a valid \"two letter\" country code as defined by  ISO 3166-1, but it isn't.");

                this.country = country;
            }

            private static bool IsValidTwoLetterCode(string code) => twoLetterCodes.Contains(code);

            private static HashSet<string> ValidTwoLetterCodes()
            {
                string[] codes = new string[] { "AF", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BV", "BR", "IO", "BN", "BG", "BF", "BI", "CV", "KH", "CM", "CA", "KY", "CF", "TD", "CL", "CN", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MO", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "MK", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SX", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VG", "VI", "WF", "EH", "YE", "ZM", "ZW", "AX" };
                return new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);
            }

            public override string ToString()
            {
                string contactData = $"{(AddressType.StructuredAddress == adrType ? "S" : "K")}{br}"; //AdrTp
                contactData += name.Replace("\n", "") + br; //Name
                contactData += (!string.IsNullOrEmpty(streetOrAddressline1) ? streetOrAddressline1.Replace("\n", "") : string.Empty) + br; //StrtNmOrAdrLine1
                contactData += (!string.IsNullOrEmpty(houseNumberOrAddressline2) ? houseNumberOrAddressline2.Replace("\n", "") : string.Empty) + br; //BldgNbOrAdrLine2
                contactData += zipCode.Replace("\n", "") + br; //PstCd
                contactData += city.Replace("\n", "") + br; //TwnNm
                contactData += country + br; //Ctry
                return contactData;
            }

            public enum AddressType
            {
                StructuredAddress,
                CombinedAddress
            }

            public class SwissQrCodeContactException : Exception
            {
                public SwissQrCodeContactException()
                {
                }

                public SwissQrCodeContactException(string message)
                    : base(message)
                {
                }

                public SwissQrCodeContactException(string message, Exception inner)
                    : base(message, inner)
                {
                }
            }
        }

        public override string ToString()
        {
            //Header "logical" element
            var SwissQrCodePayload = "SPC" + br; //QRType
            SwissQrCodePayload += "0200" + br; //Version
            SwissQrCodePayload += "1" + br; //Coding

            //CdtrInf "logical" element
            SwissQrCodePayload += iban.ToString() + br; //IBAN


            //Cdtr "logical" element
            SwissQrCodePayload += creditor.ToString();

            //UltmtCdtr "logical" element
            //Since version 2.0 ultimate creditor was marked as "for future use" and has to be delivered empty in any case!
            SwissQrCodePayload += string.Concat(Enumerable.Repeat(br, 7).ToArray());

            //CcyAmtDate "logical" element
            //Amoutn has to use . as decimal seperator in any case. See https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-en.pdf page 27.
            SwissQrCodePayload += (amount != null ? $"{amount:0.00}".Replace(",", ".") : string.Empty) + br; //Amt
            SwissQrCodePayload += currency + br; //Ccy                
            //Removed in S-QR version 2.0
            //SwissQrCodePayload += (requestedDateOfPayment != null ?  ((DateTime)requestedDateOfPayment).ToString("yyyy-MM-dd") : string.Empty) + br; //ReqdExctnDt

            //UltmtDbtr "logical" element
            if (debitor != null)
                SwissQrCodePayload += debitor.ToString();
            else
                SwissQrCodePayload += string.Concat(Enumerable.Repeat(br, 7).ToArray());


            //RmtInf "logical" element
            SwissQrCodePayload += reference.RefType.ToString() + br; //Tp
            SwissQrCodePayload += (!string.IsNullOrEmpty(reference.ReferenceText) ? reference.ReferenceText : string.Empty) + br; //Ref


            //AddInf "logical" element
            SwissQrCodePayload += (!string.IsNullOrEmpty(additionalInformation.UnstructureMessage) ? additionalInformation.UnstructureMessage : string.Empty) + br; //Ustrd
            SwissQrCodePayload += additionalInformation.Trailer + br; //Trailer
            SwissQrCodePayload += (!string.IsNullOrEmpty(additionalInformation.BillInformation) ? additionalInformation.BillInformation : string.Empty) + br; //StrdBkgInf

            //AltPmtInf "logical" element
            if (!string.IsNullOrEmpty(alternativeProcedure1))
                SwissQrCodePayload += alternativeProcedure1.Replace("\n", "") + br; //AltPmt
            if (!string.IsNullOrEmpty(alternativeProcedure2))
                SwissQrCodePayload += alternativeProcedure2.Replace("\n", "") + br; //AltPmt

            //S-QR specification 2.0, chapter 4.2.3
            if (SwissQrCodePayload.EndsWith(br))
                SwissQrCodePayload = SwissQrCodePayload.Remove(SwissQrCodePayload.Length - br.Length);

            return SwissQrCodePayload;
        }

        /// <summary>
        /// ISO 4217 currency codes.
        /// </summary>
        public enum Currency
        {
            CHF = 756,
            EUR = 978
        }

        public class SwissQrCodeException : Exception
        {
            public SwissQrCodeException()
            {
            }

            public SwissQrCodeException(string message)
                : base(message)
            {
            }

            public SwissQrCodeException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }

    internal class ShadowSocksConfig : Payload
    {
        private readonly string hostname, password, tag, methodStr, parameter;
        private readonly Method method;
        private readonly int port;
        private Dictionary<string, string> encryptionTexts = new Dictionary<string, string>() {
            { "Chacha20IetfPoly1305", "chacha20-ietf-poly1305" },
            { "Aes128Gcm", "aes-128-gcm" },
            { "Aes192Gcm", "aes-192-gcm" },
            { "Aes256Gcm", "aes-256-gcm" },

            { "XChacha20IetfPoly1305", "xchacha20-ietf-poly1305" },

            { "Aes128Cfb", "aes-128-cfb" },
            { "Aes192Cfb", "aes-192-cfb" },
            { "Aes256Cfb", "aes-256-cfb" },
            { "Aes128Ctr", "aes-128-ctr" },
            { "Aes192Ctr", "aes-192-ctr" },
            { "Aes256Ctr", "aes-256-ctr" },
            { "Camellia128Cfb", "camellia-128-cfb" },
            { "Camellia192Cfb", "camellia-192-cfb" },
            { "Camellia256Cfb", "camellia-256-cfb" },
            { "Chacha20Ietf", "chacha20-ietf" },

            { "Aes256Cb", "aes-256-cfb" },

            { "Aes128Ofb", "aes-128-ofb" },
            { "Aes192Ofb", "aes-192-ofb" },
            { "Aes256Ofb", "aes-256-ofb" },
            { "Aes128Cfb1", "aes-128-cfb1" },
            { "Aes192Cfb1", "aes-192-cfb1" },
            { "Aes256Cfb1", "aes-256-cfb1" },
            { "Aes128Cfb8", "aes-128-cfb8" },
            { "Aes192Cfb8", "aes-192-cfb8" },
            { "Aes256Cfb8", "aes-256-cfb8" },

            { "Chacha20", "chacha20" },
            { "BfCfb", "bf-cfb" },
            { "Rc4Md5", "rc4-md5" },
            { "Salsa20", "salsa20" },

            { "DesCfb", "des-cfb" },
            { "IdeaCfb", "idea-cfb" },
            { "Rc2Cfb", "rc2-cfb" },
            { "Cast5Cfb", "cast5-cfb" },
            { "Salsa20Ctr", "salsa20-ctr" },
            { "Rc4", "rc4" },
            { "SeedCfb", "seed-cfb" },
            { "Table", "table" }
        };

        /// <summary>
        /// Generates a ShadowSocks proxy config payload.
        /// </summary>
        /// <param name="hostname">Hostname of the ShadowSocks proxy</param>
        /// <param name="port">Port of the ShadowSocks proxy</param>
        /// <param name="password">Password of the SS proxy</param>
        /// <param name="method">Encryption type</param>
        /// <param name="tag">Optional tag line</param>
        public ShadowSocksConfig(string hostname, int port, string password, Method method, string? tag = null)
            : this(hostname, port, password, method, null, tag)
        {
        }

        public ShadowSocksConfig(string hostname, int port, string password, Method method, string plugin, string pluginOption, string? tag = null)
            : this(hostname, port, password, method, new Dictionary<string, string>
            {
                ["plugin"] = plugin + (
                string.IsNullOrEmpty(pluginOption)
                ? ""
                : $";{pluginOption}"
            )
            }, tag)
        {
        }

        private Dictionary<string, string> UrlEncodeTable = new Dictionary<string, string>
        {
            [" "] = "+",
            ["\0"] = "%00",
            ["\t"] = "%09",
            ["\n"] = "%0a",
            ["\r"] = "%0d",
            ["\""] = "%22",
            ["#"] = "%23",
            ["$"] = "%24",
            ["%"] = "%25",
            ["&"] = "%26",
            ["'"] = "%27",
            ["+"] = "%2b",
            [","] = "%2c",
            ["/"] = "%2f",
            [":"] = "%3a",
            [";"] = "%3b",
            ["<"] = "%3c",
            ["="] = "%3d",
            [">"] = "%3e",
            ["?"] = "%3f",
            ["@"] = "%40",
            ["["] = "%5b",
            ["\\"] = "%5c",
            ["]"] = "%5d",
            ["^"] = "%5e",
            ["`"] = "%60",
            ["{"] = "%7b",
            ["|"] = "%7c",
            ["}"] = "%7d",
            ["~"] = "%7e",
        };

        private string UrlEncode(string i)
        {
            string j = i;
            foreach (var kv in UrlEncodeTable)
            {
                j = j.Replace(kv.Key, kv.Value);
            }

            return j;
        }

        public ShadowSocksConfig(string hostname, int port, string password, Method method, Dictionary<string, string> parameters, string tag = null)
        {
            this.hostname = Uri.CheckHostName(hostname) == UriHostNameType.IPv6
                ? $"[{hostname}]"
                : hostname;
            if (port < 1 || port > 65535)
                throw new ShadowSocksConfigException("Value of 'port' must be within 0 and 65535.");
            this.port = port;
            this.password = password;
            this.method = method;
            methodStr = encryptionTexts[method.ToString()];
            this.tag = tag;

            if (parameters != null)
                parameter =
                    string.Join("&",
                    parameters.Select(
                        kv => $"{UrlEncode(kv.Key)}={UrlEncode(kv.Value)}"
                    ).ToArray());
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(parameter))
            {
                var connectionString = $"{methodStr}:{password}@{hostname}:{port}";
                var connectionStringEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(connectionString));
                return $"ss://{connectionStringEncoded}{(!string.IsNullOrEmpty(tag) ? $"#{tag}" : string.Empty)}";
            }
            var authString = $"{methodStr}:{password}";
            var authStringEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString))
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
            return $"ss://{authStringEncoded}@{hostname}:{port}/?{parameter}{(!string.IsNullOrEmpty(tag) ? $"#{tag}" : string.Empty)}";
        }

        public enum Method
        {
            // AEAD
            Chacha20IetfPoly1305,
            Aes128Gcm,
            Aes192Gcm,
            Aes256Gcm,

            // AEAD, not standard
            XChacha20IetfPoly1305,

            // Stream cipher
            Aes128Cfb,
            Aes192Cfb,
            Aes256Cfb,
            Aes128Ctr,
            Aes192Ctr,
            Aes256Ctr,
            Camellia128Cfb,
            Camellia192Cfb,
            Camellia256Cfb,
            Chacha20Ietf,

            // alias of Aes256Cfb
            Aes256Cb,

            // Stream cipher, not standard
            Aes128Ofb,
            Aes192Ofb,
            Aes256Ofb,
            Aes128Cfb1,
            Aes192Cfb1,
            Aes256Cfb1,
            Aes128Cfb8,
            Aes192Cfb8,
            Aes256Cfb8,

            // Stream cipher, deprecated
            Chacha20,
            BfCfb,
            Rc4Md5,
            Salsa20,

            // Not standard and not in acitve use
            DesCfb,
            IdeaCfb,
            Rc2Cfb,
            Cast5Cfb,
            Salsa20Ctr,
            Rc4,
            SeedCfb,
            Table
        }

        public class ShadowSocksConfigException : Exception
        {
            public ShadowSocksConfigException()
            {
            }

            public ShadowSocksConfigException(string message)
                : base(message)
            {
            }

            public ShadowSocksConfigException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }

    internal class MoneroTransaction : Payload
    {
        private readonly string address, txPaymentId, recipientName, txDescription;
        private readonly float? txAmount;

        /// <summary>
        /// Creates a monero transaction payload
        /// </summary>
        /// <param name="address">Receiver's monero address</param>
        /// <param name="txAmount">Amount to transfer</param>
        /// <param name="txPaymentId">Payment id</param>
        /// <param name="recipientName">Receipient's name</param>
        /// <param name="txDescription">Reference text / payment description</param>
        public MoneroTransaction(string address, float? txAmount = null, string txPaymentId = null, string recipientName = null, string txDescription = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new MoneroTransactionException("The address is mandatory and has to be set.");
            this.address = address;
            if (txAmount != null && txAmount <= 0)
                throw new MoneroTransactionException("Value of 'txAmount' must be greater than 0.");
            this.txAmount = txAmount;
            this.txPaymentId = txPaymentId;
            this.recipientName = recipientName;
            this.txDescription = txDescription;
        }

        public override string ToString()
        {
            var moneroUri = $"monero://{address}{(!string.IsNullOrEmpty(txPaymentId) || !string.IsNullOrEmpty(recipientName) || !string.IsNullOrEmpty(txDescription) || txAmount != null ? "?" : string.Empty)}";
            moneroUri += !string.IsNullOrEmpty(txPaymentId) ? $"tx_payment_id={Uri.EscapeDataString(txPaymentId)}&" : string.Empty;
            moneroUri += !string.IsNullOrEmpty(recipientName) ? $"recipient_name={Uri.EscapeDataString(recipientName)}&" : string.Empty;
            moneroUri += txAmount != null ? $"tx_amount={txAmount.ToString().Replace(",", ".")}&" : string.Empty;
            moneroUri += !string.IsNullOrEmpty(txDescription) ? $"tx_description={Uri.EscapeDataString(txDescription)}" : string.Empty;
            return moneroUri.TrimEnd('&');
        }

        public class MoneroTransactionException : Exception
        {
            public MoneroTransactionException()
            {
            }

            public MoneroTransactionException(string message)
                : base(message)
            {
            }

            public MoneroTransactionException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }

    internal class SlovenianUpnQr : Payload
    {
        // Keep in mind, that the ECC level has to be set to "M", version to 15 and ECI to EciMode.Iso8859_2 when generating a SlovenianUpnQr!
        // SlovenianUpnQr specification: https://www.upn-qr.si/uploads/files/NavodilaZaProgramerjeUPNQR.pdf

        private string _payerName = "";
        private string _payerAddress = "";
        private string _payerPlace = "";
        private string _amount = "";
        private string _code = "";
        private string _purpose = "";
        private string _deadLine = "";
        private string _recipientIban = "";
        private string _recipientName = "";
        private string _recipientAddress = "";
        private string _recipientPlace = "";
        private string _recipientSiModel = "";
        private string _recipientSiReference = "";

        public override int Version { get { return 15; } }
        public override QRCodeGenerator.ECCLevel EccLevel { get { return QRCodeGenerator.ECCLevel.M; } }
        public override QRCodeGenerator.EciMode EciMode { get { return QRCodeGenerator.EciMode.Iso8859_2; } }

        private string LimitLength(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public SlovenianUpnQr(string payerName, string payerAddress, string payerPlace, string recipientName, string recipientAddress, string recipientPlace, string recipientIban, string description, double amount, string recipientSiModel = "SI00", string recipientSiReference = "", string code = "OTHR") :
            this(payerName, payerAddress, payerPlace, recipientName, recipientAddress, recipientPlace, recipientIban, description, amount, null, recipientSiModel, recipientSiReference, code)
        { }

        public SlovenianUpnQr(string payerName, string payerAddress, string payerPlace, string recipientName, string recipientAddress, string recipientPlace, string recipientIban, string description, double amount, DateTime? deadline, string recipientSiModel = "SI99", string recipientSiReference = "", string code = "OTHR")
        {
            _payerName = LimitLength(payerName.Trim(), 33);
            _payerAddress = LimitLength(payerAddress.Trim(), 33);
            _payerPlace = LimitLength(payerPlace.Trim(), 33);
            _amount = FormatAmount(amount);
            _code = LimitLength(code.Trim().ToUpper(), 4);
            _purpose = LimitLength(description.Trim(), 42);
            _deadLine = deadline == null ? "" : deadline?.ToString("dd.MM.yyyy");
            _recipientIban = LimitLength(recipientIban.Trim(), 34);
            _recipientName = LimitLength(recipientName.Trim(), 33);
            _recipientAddress = LimitLength(recipientAddress.Trim(), 33);
            _recipientPlace = LimitLength(recipientPlace.Trim(), 33);
            _recipientSiModel = LimitLength(recipientSiModel.Trim().ToUpper(), 4);
            _recipientSiReference = LimitLength(recipientSiReference.Trim(), 22);
        }

        private string FormatAmount(double amount)
        {
            int _amt = (int)Math.Round(amount * 100.0);
            return string.Format("{0:00000000000}", _amt);
        }

        private int CalculateChecksum()
        {
            int _cs = 5 + _payerName.Length; // 5 = UPNQR constant Length
            _cs += _payerAddress.Length;
            _cs += _payerPlace.Length;
            _cs += _amount.Length;
            _cs += _code.Length;
            _cs += _purpose.Length;
            _cs += _deadLine.Length;
            _cs += _recipientIban.Length;
            _cs += _recipientName.Length;
            _cs += _recipientAddress.Length;
            _cs += _recipientPlace.Length;
            _cs += _recipientSiModel.Length;
            _cs += _recipientSiReference.Length;
            _cs += 19;
            return _cs;
        }

        public override string ToString()
        {
            var _sb = new StringBuilder();
            _sb.Append("UPNQR");
            _sb.Append('\n').Append('\n').Append('\n').Append('\n').Append('\n');
            _sb.Append(_payerName).Append('\n');
            _sb.Append(_payerAddress).Append('\n');
            _sb.Append(_payerPlace).Append('\n');
            _sb.Append(_amount).Append('\n').Append('\n').Append('\n');
            _sb.Append(_code.ToUpper()).Append('\n');
            _sb.Append(_purpose).Append('\n');
            _sb.Append(_deadLine).Append('\n');
            _sb.Append(_recipientIban.ToUpper()).Append('\n');
            _sb.Append(_recipientSiModel).Append(_recipientSiReference).Append('\n');
            _sb.Append(_recipientName).Append('\n');
            _sb.Append(_recipientAddress).Append('\n');
            _sb.Append(_recipientPlace).Append('\n');
            _sb.AppendFormat("{0:000}", CalculateChecksum()).Append('\n');
            return _sb.ToString();
        }
    }

    public static bool ChecksumMod10(string digits)
    {
        if (string.IsNullOrEmpty(digits) || digits.Length < 2)
            return false;
        int[] mods = new int[] { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };

        int remainder = 0;
        for (int i = 0; i < digits.Length - 1; i++)
        {
            var num = Convert.ToInt32(digits[i]) - 48;
            remainder = mods[(num + remainder) % 10];
        }

        var checksum = (10 - remainder) % 10;
        return checksum == Convert.ToInt32(digits[digits.Length - 1]) - 48;
    }
}
