using System.Text;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

internal class ContactData : Payload
{
    private readonly string _firstname;
    private readonly string _lastname;
    private readonly string? _nickname;
    private readonly string? _phone;
    private readonly string? mobilePhone;
    private readonly string? workPhone;
    private readonly string? email;
    private readonly DateTime? birthday;
    private readonly string? website;
    private readonly string street;
    private readonly string houseNumber;
    private readonly string city;
    private readonly string zipCode;
    private readonly string stateRegion;
    private readonly string country;
    private readonly string note;
    private readonly ContactOutputType outputType;
    private readonly AddressOrder addressOrder;

    /// <summary>
    /// Generates a vCard or meCard contact dataset.
    /// </summary>
    /// <param name="outputType">Payload output type.</param>
    /// <param name="firstname">The firstname.</param>
    /// <param name="lastname">The lastname.</param>
    /// <param name="nickname">The displayname.</param>
    /// <param name="phone">Normal phone number.</param>
    /// <param name="mobilePhone">Mobile phone.</param>
    /// <param name="workPhone">Office phone number.</param>
    /// <param name="email">E-Mail address.</param>
    /// <param name="birthday">Birthday.</param>
    /// <param name="website">Website / Homepage.</param>
    /// <param name="street">Street.</param>
    /// <param name="houseNumber">Housenumber.</param>
    /// <param name="city">City.</param>
    /// <param name="stateRegion">State or Region.</param>
    /// <param name="zipCode">Zip code.</param>
    /// <param name="country">Country.</param>
    /// <param name="addressOrder">The address order format to use.</param>
    /// <param name="note">Memo text / notes.</param>
    public ContactData(
        ContactOutputType outputType,
        string firstname,
        string lastname,
        string? nickname = null,
        string? phone = null,
        string? mobilePhone = null,
        string? workPhone = null,
        string? email = null,
        DateTime? birthday = null,
        string? website = null,
        string? street = null,
        string? houseNumber = null,
        string? city = null,
        string? zipCode = null,
        string? country = null,
        string? note = null,
        string? stateRegion = null,
        AddressOrder addressOrder = AddressOrder.Default)
    {
        _firstname = firstname;
        _lastname = lastname;
        _nickname = nickname;
        this._phone = phone;
        this.mobilePhone = mobilePhone;
        this.workPhone = workPhone;
        this.email = email;
        this.birthday = birthday;
        this.website = website;
        this.street = street;
        this.houseNumber = houseNumber;
        this.city = city;
        this.stateRegion = stateRegion;
        this.zipCode = zipCode;
        this.country = country;
        this.addressOrder = addressOrder;
        this.note = note;
        this.outputType = outputType;
    }

    public override string ToString()
    {
        StringBuilder payload = new StringBuilder(100);
        if (outputType == ContactOutputType.MeCard)
        {
            payload.Append("MECARD+\r\n");
            if (!string.IsNullOrEmpty(_firstname) && !string.IsNullOrEmpty(_lastname))
                payload.Append($"N:{_lastname}, {_firstname}\r\n");
            else if (!string.IsNullOrEmpty(_firstname) || !string.IsNullOrEmpty(_lastname))
                payload.Append($"N:{_firstname}{_lastname}\r\n");
            if (!string.IsNullOrEmpty(_phone))
                payload.Append($"TEL:{_phone}\r\n");
            if (!string.IsNullOrEmpty(mobilePhone))
                payload.Append($"TEL:{mobilePhone}\r\n");
            if (!string.IsNullOrEmpty(workPhone))
                payload.Append($"TEL:{workPhone}\r\n");
            if (!string.IsNullOrEmpty(email))
                payload.Append($"EMAIL:{email}\r\n");
            if (!string.IsNullOrEmpty(note))
                payload.Append($"NOTE:{note}\r\n");
            if (birthday != null)
                payload.Append($"BDAY:{((DateTime)birthday).ToString("yyyyMMdd")}\r\n");
            string addressString = string.Empty;

            if (addressOrder == AddressOrder.Default)
            {
                addressString = $"ADR:,,{(!string.IsNullOrEmpty(street) ? street + " " : string.Empty)}{(!string.IsNullOrEmpty(houseNumber) ? houseNumber : string.Empty)},{(!string.IsNullOrEmpty(zipCode) ? zipCode : "")},{(!string.IsNullOrEmpty(city) ? city : "")},{(!string.IsNullOrEmpty(stateRegion) ? stateRegion : "")},{(!string.IsNullOrEmpty(country) ? country : "")}\r\n";
            }
            else
            {
                addressString = $"ADR:,,{(!string.IsNullOrEmpty(houseNumber) ? houseNumber + " " : string.Empty)}{(!string.IsNullOrEmpty(street) ? street : string.Empty)},{(!string.IsNullOrEmpty(city) ? city : "")},{(!string.IsNullOrEmpty(stateRegion) ? stateRegion : "")},{(!string.IsNullOrEmpty(zipCode) ? zipCode : "")},{(!string.IsNullOrEmpty(country) ? country : "")}\r\n";
            }

            payload.Append(addressString);
            if (!string.IsNullOrEmpty(website))
                payload.Append($"URL:{website}\r\n");
            if (!string.IsNullOrEmpty(_nickname))
                payload.Append($"NICKNAME:{_nickname}\r\n");

            string tmp = payload.ToString().Trim(new char[] { '\r', '\n' });
            payload = new StringBuilder(tmp);
        }
        else
        {
            var version = outputType.ToString().Substring(5);
            if (version.Length > 1)
                version = version.Insert(1, ".");
            else
                version += ".0";

            payload.Append("BEGIN:VCARD\r\n");
            payload.Append($"VERSION:{version}\r\n");

            payload.Append($"N:{(!string.IsNullOrEmpty(_lastname) ? _lastname : "")};{(!string.IsNullOrEmpty(_firstname) ? _firstname : "")};;;\r\n");
            payload.Append($"FN:{(!string.IsNullOrEmpty(_firstname) ? _firstname + " " : "")}{(!string.IsNullOrEmpty(_lastname) ? _lastname : "")}\r\n");

            if (!string.IsNullOrEmpty(_phone))
            {
                payload.Append($"TEL;");
                if (outputType == ContactOutputType.VCard21)
                    payload.Append($"HOME;VOICE:{_phone}");
                else if (outputType == ContactOutputType.VCard3)
                    payload.Append($"TYPE=HOME,VOICE:{_phone}");
                else
                    payload.Append($"TYPE=home,voice;VALUE=uri:tel:{_phone}");
                payload.Append("\r\n");
            }

            if (!string.IsNullOrEmpty(mobilePhone))
            {
                payload.Append($"TEL;");
                if (outputType == ContactOutputType.VCard21)
                    payload.Append($"HOME;CELL:{mobilePhone}");
                else if (outputType == ContactOutputType.VCard3)
                    payload.Append($"TYPE=HOME,CELL:{mobilePhone}");
                else
                    payload.Append($"TYPE=home,cell;VALUE=uri:tel:{mobilePhone}");
                payload.Append("\r\n");
            }

            if (!string.IsNullOrEmpty(workPhone))
            {
                payload.Append($"TEL;");
                if (outputType == ContactOutputType.VCard21)
                    payload.Append($"WORK;VOICE:{workPhone}");
                else if (outputType == ContactOutputType.VCard3)
                    payload.Append($"TYPE=WORK,VOICE:{workPhone}");
                else
                    payload.Append($"TYPE=work,voice;VALUE=uri:tel:{workPhone}");
                payload.Append("\r\n");
            }

            payload.Append("ADR;");
            if (outputType == ContactOutputType.VCard21)
                payload.Append("HOME;PREF:");
            else if (outputType == ContactOutputType.VCard3)
                payload.Append("TYPE=HOME,PREF:");
            else
                payload.Append("TYPE=home,pref:");
            string addressString = string.Empty;

            if (addressOrder == AddressOrder.Default)
            {
                addressString = $";;{(!string.IsNullOrEmpty(street) ? street + " " : string.Empty)}{(!string.IsNullOrEmpty(houseNumber) ? houseNumber : "")};{(!string.IsNullOrEmpty(zipCode) ? zipCode : "")};{(!string.IsNullOrEmpty(city) ? city : "")};{(!string.IsNullOrEmpty(stateRegion) ? stateRegion : "")};{(!string.IsNullOrEmpty(country) ? country : "")}\r\n";
            }
            else
            {
                addressString = $";;{(!string.IsNullOrEmpty(houseNumber) ? houseNumber + " " : string.Empty)}{(!string.IsNullOrEmpty(street) ? street : "")};{(!string.IsNullOrEmpty(city) ? city : "")};{(!string.IsNullOrEmpty(stateRegion) ? stateRegion : "")};{(!string.IsNullOrEmpty(zipCode) ? zipCode : "")};{(!string.IsNullOrEmpty(country) ? country : "")}\r\n";
            }

            payload.Append(addressString);

            if (birthday != null)
                payload.Append($"BDAY:{((DateTime)birthday).ToString("yyyyMMdd")}\r\n");
            if (!string.IsNullOrEmpty(website))
                payload.Append($"URL:{website}\r\n");
            if (!string.IsNullOrEmpty(email))
                payload.Append($"EMAIL:{email}\r\n");
            if (!string.IsNullOrEmpty(note))
                payload.Append($"NOTE:{note}\r\n");
            if (outputType != ContactOutputType.VCard21 && !string.IsNullOrEmpty(_nickname))
                payload.Append($"NICKNAME:{_nickname}\r\n");

            payload.Append("END:VCARD");
        }

        return payload.ToString();
    }

    /// <summary>
    /// Possible output types. Either vCard 2.1, vCard 3.0, vCard 4.0 or MeCard.
    /// </summary>
    public enum ContactOutputType
    {
        /// <summary>
        /// MeCard format. (Default)
        /// </summary>
        MeCard,

        /// <summary>
        /// vCard 2.1 format.
        /// </summary>
        VCard21,

        /// <summary>
        /// vCard 3.0 format.
        /// </summary>
        VCard3,

        /// <summary>
        /// vCard 4.0 format.
        /// </summary>
        VCard4
    }

    /// <summary>
    /// define the address format
    /// Default: European format, ([Street] [House Number] and [Postal Code] [City]
    /// Reversed: North American and others format ([House Number] [Street] and [City] [Postal Code]).
    /// </summary>
    public enum AddressOrder
    {
        /// <summary>
        /// Default: European format, ([Street] [House Number] and [Postal Code] [City]
        /// </summary>
        Default,

        /// <summary>
        /// Reversed: North American and others format ([House Number] [Street] and [City] [Postal Code]).
        /// </summary>
        Reversed
    }
}

