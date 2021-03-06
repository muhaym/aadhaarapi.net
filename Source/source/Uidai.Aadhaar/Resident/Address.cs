﻿#region Copyright
/********************************************************************************
 * Aadhaar API for .NET
 * Copyright © 2015 Souvik Dey Chowdhury
 * 
 * This file is part of Aadhaar API for .NET.
 * 
 * Aadhaar API for .NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 * 
 * Aadhaar API for .NET is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with Aadhaar API for .NET. If not, see http://www.gnu.org/licenses.
 ********************************************************************************/
#endregion

using System;
using System.Text;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents address of a resident in parts.
    /// </summary>
    public class Address : IUsed, IXml
    {
        private string pincode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        public Address() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public Address(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the care of.
        /// </summary>
        public string CareOf { get; set; }

        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        public string House { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the landmark.
        /// </summary>
        public string Landmark { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the village/town/city.
        /// </summary>
        public string VillageOrCity { get; set; }

        /// <summary>
        /// Gets or sets the sub-district.
        /// </summary>
        public string SubDistrict { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the pincode. 
        /// </summary>
        public string Pincode
        {
            get { return pincode; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !AadhaarHelper.ValidatePincode(value))
                    throw new ArgumentException(InvalidPincode, nameof(Pincode));
                pincode = value;
            }
        }

        /// <summary>
        /// Gets or sets the post office.
        /// </summary>
        public string PostOffice { get; set; }

        /// <summary>
        /// Determines whether a particular resident data is used or not.
        /// </summary>
        /// <returns>true if the data is used; otherwise, false.</returns>
        public bool IsUsed() => !(string.IsNullOrWhiteSpace(CareOf) &&
                                  string.IsNullOrWhiteSpace(House) &&
                                  string.IsNullOrWhiteSpace(Street) &&
                                  string.IsNullOrWhiteSpace(Landmark) &&
                                  string.IsNullOrWhiteSpace(Locality) &&
                                  string.IsNullOrWhiteSpace(VillageOrCity) &&
                                  string.IsNullOrWhiteSpace(SubDistrict) &&
                                  string.IsNullOrWhiteSpace(District) &&
                                  string.IsNullOrWhiteSpace(State) &&
                                  string.IsNullOrWhiteSpace(Pincode) &&
                                  string.IsNullOrWhiteSpace(PostOffice));

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            CareOf = element.Attribute("co")?.Value;
            House = element.Attribute("house")?.Value;
            Street = element.Attribute("street")?.Value;
            Landmark = element.Attribute("lm")?.Value;
            Locality = element.Attribute("loc")?.Value;
            VillageOrCity = element.Attribute("vtc")?.Value;
            SubDistrict = element.Attribute("subdist")?.Value;
            District = element.Attribute("dist")?.Value;
            State = element.Attribute("state")?.Value;
            Pincode = element.Attribute("pc")?.Value;
            PostOffice = element.Attribute("po")?.Value;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            var address = new XElement(elementName,
                new XAttribute("co", CareOf ?? string.Empty),
                new XAttribute("house", House ?? string.Empty),
                new XAttribute("street", Street ?? string.Empty),
                new XAttribute("lm", Landmark ?? string.Empty),
                new XAttribute("loc", Locality ?? string.Empty),
                new XAttribute("vtc", VillageOrCity ?? string.Empty),
                new XAttribute("subdist", SubDistrict ?? string.Empty),
                new XAttribute("dist", District ?? string.Empty),
                new XAttribute("state", State ?? string.Empty),
                new XAttribute("pc", Pincode ?? string.Empty),
                new XAttribute("po", PostOffice ?? string.Empty));

            address.RemoveEmptyAttributes();

            return address;
        }

        /// <summary>
        /// Returns a string that represents the address as per UIDAI demographic standards.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            /* KYR Demographic Standard:
             * [C/o Person Name]
             * Building
             * [Street]
             * [Landmark], [Locality]
             * Village/Town/City, District
             * State – Pin Code
            */

            var builder = new StringBuilder(250);

            // Address Line 1
            if (!string.IsNullOrWhiteSpace(CareOf))
                builder.AppendLine($"C/o {CareOf}");

            // Address Line 2
            builder.AppendLine(House);

            // Address Line 3
            if (!string.IsNullOrWhiteSpace(Street))
                builder.AppendLine(Street);

            // Address Line 4
            if (!string.IsNullOrWhiteSpace(Landmark) || !string.IsNullOrWhiteSpace(Locality))
                builder.AppendLine($"{Landmark}, {Locality}".Trim(',', ' '));

            // Address Line 5
            builder.AppendLine($"{VillageOrCity}, {District}");

            // Address Line 6
            builder.AppendLine($"{State} – {Pincode}".Trim('–', ' '));

            return builder.ToString();
        }
    }
}