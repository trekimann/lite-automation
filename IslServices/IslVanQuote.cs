using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IslServices.WsIslPrd;
using IslServices.VanQuoteServiceDevWarr;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.IO;

namespace IslServices
{
    public class IslVanQuote
    {
        public VanQuoteDetails QuoteDetails { get; private set; }
        public string HDquoteNumber { get; private set; }
        public string HEquoteNumber { get; private set; }
        public string HPquoteNumber { get; private set; }
        public string PCquoteNumber { get; private set; }
        public string IPquoteNumber { get; private set; }
        public string PostCode { get; private set; }
        public bool QuotesReturned { get; private set; } = false;
        public string HDdeeplink { get; private set; }
        public string HEdeeplink { get; private set; }
        public string HPdeeplink { get; private set; }
        public string IPdeeplink { get; private set; }
        public string PCdeeplink { get; private set; }

        public IslVanQuote(VanQuoteDetails quoteDetails)
        {
            QuoteDetails = quoteDetails;
        }

        public void GetQuote()
        {
            var request = new getAggregatorVanQuoteReq
            {
                VanQuoteRequest = new VanQuoteRequest
                {
                    quoteHeader = GetVanQuoteRequestQuoteHeader(),
                    VanQuote = GetQuoteRequestVanQuote()
                }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(request.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, request);
                var SOAP = textWriter.ToString();
            }

            try
            {
                var api = new CVQuoteEngineAPI { SoapVersion = SoapProtocolVersion.Soap11 };
                api.getAggregatorVanQuoteCompleted += ApiGetAggregatorVanQuoteCompleted;                

                api.getAggregatorVanQuoteAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ApiGetAggregatorVanQuoteCompleted(object sender, getAggregatorVanQuoteCompletedEventArgs e)
        {
            var quotes = e.Result.BrokerResponse.quote.OrderBy(quote => quote.quoteReference).ToList();

            foreach (var quote in quotes)
            {
                string brand = quote.brandID;

                if (brand.Equals("HD"))
                {
                    HDquoteNumber = quote.quoteReference;
                    HDdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("HE"))
                {
                    HEquoteNumber = quote.quoteReference;
                    HEdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("HP"))
                {
                    HPquoteNumber = quote.quoteReference;
                    HPdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("PC"))
                {
                    PCquoteNumber = quote.quoteReference;
                    PCdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("IP"))
                {
                    IPquoteNumber = quote.quoteReference;
                    IPdeeplink = quote.deepLinkUrl;
                }
            }
            QuotesReturned = true;
        }

        public VanQuoteRequestVanQuote GetQuoteRequestVanQuote()
        {
            var vanQuoteRequest = new VanQuoteRequest
            {

                VanQuote = new VanQuoteRequestVanQuote
                {
                    autoRenewal = true,
                    marketingPermissions = false,
                    noOfVehiclesHousehold = QuoteDetails._noOfVehiclesHousehold,
                    noOfDriversinHouse = new VanQuoteRequestVanQuoteNoOfDriversinHouse { typeListName = "NoOfDrivers_Ext", noOfDriversinHouse = QuoteDetails._noOfDriversinHouse },
                    inceptionDate = QuoteDetails._inceptionDate,
                    van = GetVanDetails(),
                    drivers = GetDriversDetails(),
                    extraQuestions = GetExtraQuestionsDetails()
                }
            };

            return vanQuoteRequest.VanQuote;
        }

        private static VanQuoteRequestVanQuoteExtraQuestions GetExtraQuestionsDetails()
        {
            return new VanQuoteRequestVanQuoteExtraQuestions
            {
                carInsured = false,
                renewalCarDue = DateTime.Today,
                bikeInsured = false,
                renewalBikeDue = DateTime.Today,
                homeInsured = false,
                renewalPrice = "204",
                bestPrice = "304"
            };
        }

        private VanQuoteRequestVanQuoteDrivers GetDriversDetails()
        {
            var drivers = new VanQuoteRequestVanQuoteDrivers { policyholder = GetPolicyHolderDeatils() };

            return drivers;
        }

        private VanQuoteRequestVanQuoteDriversPolicyholder GetPolicyHolderDeatils()
        {
            primaryPhone phone = primaryPhone.home;
            if (QuoteDetails._primaryPhone.Equals("mobile"))
            {
                phone = primaryPhone.mobile;
            }
            else if (QuoteDetails._primaryPhone.Equals("work"))
            {
                phone = primaryPhone.work;
            }

            var policyHolder = new VanQuoteRequestVanQuoteDriversPolicyholder
            {
                name = new nameType
                {
                    fullName =
                        new nameTypeFullName
                        {
                            firstName = QuoteDetails._forename,
                            lastName = QuoteDetails._surname,
                            title = new nameTypeFullNameTitle { title = "003_Mr", typeListName = "NamePrefix" },
                            titleDesc = "title values"
                        }
                },
                driverId = "1",
                dateOfBirth = Convert.ToDateTime(QuoteDetails._dateOfBirthAsString),
                maritalStatus = new VanQuoteRequestVanQuoteDriversPolicyholderMaritalStatus
                {
                    maritalStatus = "B",
                    typeListName = "MaritalStatus"
                },
                maritalStatusDesc = "marital status values",
                address = GetAddressDetails(),
                ukResident = Convert.ToDateTime(QuoteDetails._dateOfBirthAsString),
                marketingPreference = GetMarketingPreferenceDetails(),
                employment = GetEmploymentDetails(),
                licence = GetLicenceDetails(),
                myLicenceIndicator = false,
                useOtherVehicle =
                    new VanQuoteRequestVanQuoteDriversPolicyholderUseOtherVehicle { useOtherVehicle = "own_van", typeListName = "AccessOtherVeh_Ext" },
                useOtherVehicleDesc = "useother values",
                primaryEmail = QuoteDetails._primaryEmail,
                secondaryEmail = QuoteDetails._secondaryEmail,
                homePhoneNumber = QuoteDetails._homePhoneNumber,
                workPhoneNumber = QuoteDetails._workPhoneNumber,
                mobilePhoneNumber = QuoteDetails._mobilePhoneNumber,
                faxPhoneNumber = QuoteDetails._faxPhoneNumber,
                primaryPhone = phone,
                medicalConditions = new VanQuoteRequestVanQuoteDriversPolicyholderMedicalConditions
                {
                    medicalConditions = "99_NO",
                    typeListName = "DVLAMedicalCondition_Ext"
                },
                medicalConditionsDesc = "medical condition values",
                nonMotoringConvictions = QuoteDetails._nonMotoringConvictions,
                isPolicyDeclinedForDrivers = QuoteDetails._isPolicyDeclinedForDrivers
            };

            return policyHolder;
        }

        private licenceType GetLicenceDetails()
        {
            return new licenceType
            {
                number = QuoteDetails._drivingLicenceNo,
                type = new licenceTypeType { type = "F_FM", typeListName = "LicenceType_Ext" },
                typeDesc = "licencetype values",
                lengthHeld = Convert.ToDateTime(QuoteDetails._drivingLicenceHeldSinceAsString),
                fullUkLicenceIamCert = false,
                yearPassPlusCert = false
            };
        }

        private static employmentType[] GetEmploymentDetails()
        {
            var employmentType = new employmentType[1];
            employmentType[0] = new employmentType
            {
                isPrimary = true,
                employmentStatusCode = new employmentTypeEmploymentStatusCode
                {
                    employmentStatusCode = "E",
                    typeListName = "FullTimeEmpStatus_Ext"
                },
                employmentStatusDesc = "status values",
                employmentOccupationCode = new employmentTypeEmploymentOccupationCode
                {
                    employmentOccupationCode = "A01",
                    typeListName = "OccupationType_Ext"
                },
                employmentOccupationDesc = "occupation values",
                whatWereYouStudying = "science",
                employmentBusinessCode = new employmentTypeEmploymentBusinessCode
                {
                    employmentBusinessCode = "001",
                    typeListName = "BusinessType_Ext"
                },
                employmentBusinessDesc = "business values"
            };

            return employmentType;
        }

        private VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreference GetMarketingPreferenceDetails()//was static
        {
            var fieldsData = new VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreferenceFieldsData[1];
            fieldsData[0] = new VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreferenceFieldsData
            {
                code = "test",
                value = "test"
            };

            return new VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreference
            {
                homeOwner = true,
                homeInsuranceRenewal =
                    new VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreferenceHomeInsuranceRenewal
                    {
                        homeInsuranceRenewal = "march",
                        typeListName = "InsuranceRenewalMonth_Ext"
                    },
                timeAtCurrentAddress =
                    new VanQuoteRequestVanQuoteDriversPolicyholderMarketingPreferenceTimeAtCurrentAddress
                    {
                        timeAtCurrentAddress = QuoteDetails._timeAtCurrentAddress,
                        typeListName = "NCDGrantedYears_Ext"
                    },
                anyChildrenUnder16 = QuoteDetails._anyChildrenUnder16,
                fieldsData = fieldsData
            };
        }

        private addressType GetAddressDetails()
        {
            var formattedAddress = GetAddressDetailsFromIsl();
            return new addressType
            {
                addressLine1 = formattedAddress.AddressLine1,
                addressLine2 = formattedAddress.AddressLine2,
                addressLine3 = formattedAddress.AddressLine3,
                town = formattedAddress.City,
                county = formattedAddress.County,
                postCode = formattedAddress.PostalCode
            };
        }

        private Address2 GetAddressDetailsFromIsl()
        {
            var contextData = CreateIslContextRequest();
            var data = new BasicHttpBinding_AddressSearch();

            //-------------Check if address starts with number or word
            var firstLineRegEx = new Regex(@"[0-9]");
            var firstLine = QuoteDetails._addressFirstLine;
            var startsWithNumber = firstLineRegEx.Match(QuoteDetails._addressFirstLine);
            if (startsWithNumber.Success)
                firstLine = QuoteDetails._addressFirstLine.Split(' ')[0];

            var request = new AddressSearchRequest
            {
                PostalCode = QuoteDetails._postcode,
                AddressLine1 = firstLine
            };

            var response = data.RetrieveAddressAsync(request, contextData);
            return response.ResponseContext.ResultFlag ? response.AddressSearchResult.Addresses[0] : new Address2();
        }

        public VanQuoteRequestVanQuoteVan GetVanDetails()
        {
            var vehicleDetails = GetVanDetailsFromIsl();

            return new VanQuoteRequestVanQuoteVan
            {
                abiCode = vehicleDetails.Dvla.ABI,
                registration = vehicleDetails.Dvla.RegistrationNumber,
                make = vehicleDetails.Dvla.ModelDescription,
                bodyType = vehicleDetails.Dvla.BodyCode,
                yearOfRegistration = vehicleDetails.Dvla.RegistrationDate.Year.ToString(),
                transmission = new VanQuoteRequestVanQuoteVanTransmission { transmission = "001", typeListName = "Transmission_Ext" },
                engineSize = vehicleDetails.Dvla.EngineSize,
                model = vehicleDetails.Dvla.ModelDescription,
                noOfDoors = vehicleDetails.Smmt.NumberOfDoors.ToString(),
                noOfSeats = vehicleDetails.Smmt.NumberOfSeats.ToString(),
                fuelType = vehicleDetails.Smmt.FuelType,
                value = QuoteDetails._VanValue,
                purchaseDate = QuoteDetails._purchaseDate,
                importType =
                    new VanQuoteRequestVanQuoteVanImportType
                    {
                        importType = vehicleDetails.Dvla.Imported ? "Yes" : "No",
                        typeListName = "ImportType_Ext"
                    },
                importTypeDesc = "send original description",
                rightHandDrive = true,
                immobiliser = new VanQuoteRequestVanQuoteVanImmobiliser { immobiliser = "94", typeListName = "AlarmImmobiliser_Ext" },
                immobiliserDesc = "After market immobiliser",
                alarmDesc = "After market Thatham alarm",
                tracker = true,
                trackerDesc = "GPS Tracker",
                overnightPostCode = QuoteDetails._postcode,
                parkedDaytimeData = new VanQuoteRequestVanQuoteVanParkedDaytimeData { code = "E", description = "Work car park" },
                parkedOvernight = new VanQuoteRequestVanQuoteVanParkedOvernight { parkedOvernight = "F", typeListName = "KeptOvernight_Ext" },
                parkedOvernightDesc = "Public Car Park",
                owner = new VanQuoteRequestVanQuoteVanOwner { owner = "1_PR", typeListName = "OwnerKeeper_Ext" },
                ownerDesc = "Original owner value, e.g. proposer",
                registeredKeeper = new VanQuoteRequestVanQuoteVanRegisteredKeeper { registeredKeeper = "1_PR", typeListName = "OwnerKeeper_Ext" },
                registeredKeeperDesc = "Original registered keeper value, e.g. proposer",
                cover = GetCoverDetails()
            };            
        }

        private VanQuoteRequestVanQuoteVanCover GetCoverDetails()
        {
            return new VanQuoteRequestVanQuoteVanCover
            {
                coverType = new VanQuoteRequestVanQuoteVanCoverCoverType { coverType = "comprehensive", typeListName = "CoverageCategory_Ext" },
                coverLevelDesc = "cover type values",
                classOfUse = new VanQuoteRequestVanQuoteVanCoverClassOfUse { classOfUse = "01", typeListName = "ClassOfUse_Ext" },
                classOfUseDesc = "class of use values",
                voluntaryExcess = new VanQuoteRequestVanQuoteVanCoverVoluntaryExcess { voluntaryExcess = "200", typeListName = "VolExcess_Ext" },
                voluntaryExcessDesc = "voluntary values",
                mainUser = "1",
                businessMileage = QuoteDetails._businessMileage,
                personalMileage = QuoteDetails._personalMileage,
                totalMileage = QuoteDetails._totalMileage,
                ncdGrantedYears = new VanQuoteRequestVanQuoteVanCoverNcdGrantedYears { ncdGrantedYears = QuoteDetails._ncdGrantedYears, typeListName = "NCDGrantedYears_Ext" },
                ncdGreaterZero = new VanQuoteRequestVanQuoteVanCoverNcdGreaterZero
                {
                    ncdCurrentlyProtected = true,
                    ncdCurrentlyProtectedSpecified = false,
                    howNcdEarn = new VanQuoteRequestVanQuoteVanCoverNcdGreaterZeroHowNcdEarn { howNcdEarn = "11", typeListName = "NCDEarnedFrom_Ext" },
                    howNcdEarnDesc = "Original description, e.g. Private Van Bonus",
                    ncdEarnedUk = true,
                    ncdProtect = true
                },
                insurancePaymentType =
                    new VanQuoteRequestVanQuoteVanCoverInsurancePaymentType { insurancePaymentType = "3", typeListName = "InsurancePaymentType_Ext" },
                previousPolicyExpire = DateTime.Now
            };
        }

        private VehicleLookupResult GetVanDetailsFromIsl()
        {
            var request = new VehicleLookupRequest();
            var data = new BasicHttpBinding_VehicleLookup();
            var contextData = CreateIslContextRequest();

            request.RegistrationNumber = QuoteDetails._vanRegistration;
            var response = data.RetrieveVehicleDetailsAsync(request, contextData);


            return response.ResponseContext.ResultFlag ? response.VehicleLookupResult : null;
        }

        private static RequestContext CreateIslContextRequest()
        {
            String username = Environment.UserName;
            return new RequestContext
            {
                IpAddressClient = "123456",
                IpAddressServer = "123456",
                RequestingModule = "123456",
                RequestingReference = "123456",
                RequestingUser = username.ToLower()
            };
        }

        public VanQuoteRequestQuoteHeader GetVanQuoteRequestQuoteHeader()
        {
            var headerAdditionalData = new VanQuoteRequestQuoteHeaderData[1];
            headerAdditionalData[0] = new VanQuoteRequestQuoteHeaderData
            {
                name = "test1",
                value = "test1"
            };

            var VanQuoteRequest = new VanQuoteRequest
            {
                quoteHeader = new VanQuoteRequestQuoteHeader
                {
                    aggsSource = QuoteDetails._aggSource,
                    aggsId = "SelLite_" + DateTime.Now.ToString("yyyyMMddhhmmsss"),
                    timestamp = DateTime.Now,
                    originOfBusiness = QuoteDetails._aggSource+"DefaultCampaign",
                    headerAdditionalData = headerAdditionalData
                }
            };

            return VanQuoteRequest.quoteHeader;
        }

    }
}
