using IslServices.WsIslPrd;
using IslServices.WsQhDevWarr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace IslServices
{
    public class IslCarQuote
    {
        private CarQuoteDetails quoteDetails;
        public string HDquoteNumber { get; private set; }
        public string HEquoteNumber { get; private set; }
        public string HPquoteNumber { get; private set; }
        public string PCquoteNumber { get; private set; }
        public string IPquoteNumber { get; private set; }
        public string PostCode { get; private set; }
        public bool quotesReturned { get; private set; } = false;
        public string HDdeeplink { get; private set; }
        public string HEdeeplink { get; private set; }
        public string HPdeeplink { get; private set; }
        public string IPdeeplink { get; private set; }
        public string PCdeeplink { get; private set; }

        public IslCarQuote(CarQuoteDetails quoteDetails)
        {
            this.quoteDetails = quoteDetails;
            this.PostCode = quoteDetails._postcode;
        }

        public void SetAggDetails()
        {
            //quoteDetails._aggSource;
        }

        public void GetQuote()
        {
            var request = new getAggregatorCarQuoteReq
            {
                CarQuoteRequest = new CarQuoteRequest
                {
                    quoteHeader = GetCarQuoteRequestHeader(),
                    carQuote = GetQuoteRequestCarQuote()
                }
            };

            //XmlSerializer xmlSerializer = new XmlSerializer(request.GetType());

            //using (StringWriter textWriter = new StringWriter())
            //{
            //    xmlSerializer.Serialize(textWriter, request);
            //    var SOAP = textWriter.ToString();
            //}

            try
            {
                var api = new PCQuoteEngineAPI { SoapVersion = SoapProtocolVersion.Soap11 };
                api.getAggregatorCarQuoteCompleted += ApiGetAggregatorCarQuoteCompleted;

                api.getAggregatorCarQuoteAsync(request);
            }
            catch (Exception ex)
            {

            }
        }

        public VehicleLookupResult GetCarDetailsFromIsl(string plate)//was static
        {
            quoteDetails._carRegistration = plate;
            return GetCarDetailsFromIsl();
        }

        private VehicleLookupResult GetCarDetailsFromIsl()//was static
        {
            var request = new VehicleLookupRequest();
            var data = new BasicHttpBinding_VehicleLookup();
            var contextData = CreateIslContextRequest();

            request.RegistrationNumber = quoteDetails._carRegistration;
            var response = data.RetrieveVehicleDetailsAsync(request, contextData);


            return response.ResponseContext.ResultFlag ? response.VehicleLookupResult : null;
        }

        private Address2 GetAddressDetailsFromIsl()
        {
            var contextData = CreateIslContextRequest();
            var data = new BasicHttpBinding_AddressSearch();

            //-------------Check if address starts with number or word
            var firstLineRegEx = new Regex(@"[0-9]");
            var firstLine = quoteDetails._addressFirstLine;
            var startsWithNumber = firstLineRegEx.Match(quoteDetails._addressFirstLine);
            if (startsWithNumber.Success)
                firstLine = quoteDetails._addressFirstLine.Split(' ')[0];

            var request = new AddressSearchRequest
            {
                PostalCode = quoteDetails._postcode,
                AddressLine1 = firstLine
            };

            var response = data.RetrieveAddressAsync(request, contextData);
            return response.ResponseContext.ResultFlag ? response.AddressSearchResult.Addresses[0] : new Address2();
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

        private void ApiGetAggregatorCarQuoteCompleted(object sender, getAggregatorCarQuoteCompletedEventArgs e)
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
                else if (brand.Equals("HE")){
                    HEquoteNumber = quote.quoteReference;
                    HEdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("HP")){
                    HPquoteNumber = quote.quoteReference;
                    HPdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("PC")){
                    PCquoteNumber = quote.quoteReference;
                    PCdeeplink = quote.deepLinkUrl;
                }
                else if (brand.Equals("IP")){
                    IPquoteNumber = quote.quoteReference;
                    IPdeeplink = quote.deepLinkUrl;
                }               
            }
            quotesReturned = true;
        }

        private CarQuoteRequestQuoteHeader GetCarQuoteRequestHeader()//was static
        {
            var headerAdditionalData = new CarQuoteRequestQuoteHeaderData[1];
            headerAdditionalData[0] = new CarQuoteRequestQuoteHeaderData
            {
                name = "test1",
                value = "test1"
            };

            var carQuoteRequest = new CarQuoteRequest
            {
                quoteHeader = new CarQuoteRequestQuoteHeader
                {
                    aggsSource = quoteDetails._aggSource,
                    aggsId = "SelLite_" + DateTime.Now.ToString("yyyyMMddhhmmsss"),
                    timestamp = DateTime.Now,
                    originOfBusiness = quoteDetails._aggSource + "DefaultCampaign",
                    headerAdditionalData = headerAdditionalData
                }
            };

            return carQuoteRequest.quoteHeader;
        }

        private CarQuoteRequestCarQuote GetQuoteRequestCarQuote()
        {
            var carQuoteRequest = new CarQuoteRequest
            {
                carQuote = new CarQuoteRequestCarQuote
                {
                    autoRenewal = true,
                    marketingPermissions = false,
                    noOfCarsHousehold = quoteDetails._noOfCarsHousehold,
                    noOfDriversinHouse = new CarQuoteRequestCarQuoteNoOfDriversinHouse { typeListName = "NoOfDrivers_Ext", noOfDriversinHouse = quoteDetails._noOfDriversInHouse },
                    inceptionDate = DateTime.Today,
                    car = GetCarDetails(),
                    drivers = GetDriversDetails(),
                    extraQuestions = GetExtraQuestionsDetails()
                }
            };

            return carQuoteRequest.carQuote;
        }

        private CarQuoteRequestCarQuoteCar GetCarDetails()
        {
            var vehicleDetails = GetCarDetailsFromIsl();

            return new CarQuoteRequestCarQuoteCar
            {
                abiCode = vehicleDetails.Dvla.ABI,
                registration = vehicleDetails.Dvla.RegistrationNumber,
                make = vehicleDetails.Dvla.ModelDescription,
                IsCarFitWithAEB = false,
                motDue = new CarQuoteRequestCarQuoteCarMotDue { motDue = "january", typeListName = "Months" },
                bodyType = vehicleDetails.Dvla.BodyCode,
                yearOfRegistration = vehicleDetails.Dvla.RegistrationDate.Year.ToString(),
                transmission = new CarQuoteRequestCarQuoteCarTransmission { transmission = "001", typeListName = "Transmission_Ext" },
                engineSize = vehicleDetails.Dvla.EngineSize,
                model = vehicleDetails.Dvla.ModelDescription,
                noOfDoors = vehicleDetails.Smmt.NumberOfDoors.ToString(),
                noOfSeats = vehicleDetails.Smmt.NumberOfSeats.ToString(),
                fuelType = vehicleDetails.Smmt.FuelType,
                carValue = 10000,
                purchaseDate = DateTime.Today,
                importType =
                    new CarQuoteRequestCarQuoteCarImportType
                    {
                        importType = vehicleDetails.Dvla.Imported ? "Yes" : "No",
                        typeListName = "ImportType_Ext"
                    },
                importTypeDesc = "send original description",
                rightHandDrive = true,
                immobiliser = new CarQuoteRequestCarQuoteCarImmobiliser { immobiliser = "94", typeListName = "AlarmImmobiliser_Ext" },
                immobiliserDesc = "After market immobiliser",
                alarmDesc = "After market Thatham alarm",
                tracker = true,
                trackerDesc = "GPS Tracker",
                overnightPostCode = quoteDetails._postcode,
                parkedDaytimeData = new CarQuoteRequestCarQuoteCarParkedDaytimeData { code = "E", description = "Work car park" },
                parkedOvernight = new CarQuoteRequestCarQuoteCarParkedOvernight { parkedOvernight = "F", typeListName = "KeptOvernight_Ext" },
                parkedOvernightDesc = "Public Car Park",
                owner = new CarQuoteRequestCarQuoteCarOwner { owner = "1_PR", typeListName = "OwnerKeeper_Ext" },
                ownerDesc = "Original owner value, e.g. proposer",
                registeredKeeper = new CarQuoteRequestCarQuoteCarRegisteredKeeper { registeredKeeper = "1_PR", typeListName = "OwnerKeeper_Ext" },
                registeredKeeperDesc = "Original registered keeper value, e.g. proposer",
                cover = GetCoverDetails()
            };
        }

        private CarQuoteRequestCarQuoteCarCover GetCoverDetails()//was static
        {
            return new CarQuoteRequestCarQuoteCarCover
            {
                coverType = new CarQuoteRequestCarQuoteCarCoverCoverType { coverType = "comprehensive", typeListName = "CoverageCategory_Ext" },
                coverLevelDesc = "cover type values",
                classOfUse = new CarQuoteRequestCarQuoteCarCoverClassOfUse { classOfUse = "01", typeListName = "ClassOfUse_Ext" },
                classOfUseDesc = "class of use values",
                voluntaryExcess = new CarQuoteRequestCarQuoteCarCoverVoluntaryExcess { voluntaryExcess = "200", typeListName = "VolExcess_Ext" },
                voluntaryExcessDesc = "voluntary values",
                mainUser = "1",
                businessMileage = quoteDetails._businessMileage,
                personalMileage = quoteDetails._personalMileage,
                totalMileage = quoteDetails._totalMileage,
                ncdGrantedYears = new CarQuoteRequestCarQuoteCarCoverNcdGrantedYears { ncdGrantedYears = quoteDetails._ncdGrantedYears, typeListName = "NCDGrantedYears_Ext" },
                ncdGreaterZero = new CarQuoteRequestCarQuoteCarCoverNcdGreaterZero
                {
                    ncdCurrentlyProtected = true,
                    ncdCurrentlyProtectedSpecified = false,
                    howNcdEarn = new CarQuoteRequestCarQuoteCarCoverNcdGreaterZeroHowNcdEarn { howNcdEarn = "11", typeListName = "NCDEarnedFrom_Ext" },
                    howNcdEarnDesc = "Original description, e.g. Private Car Bonus",
                    ncdEarnedUk = true,
                    ncdProtect = true
                },
                insurancePaymentType =
                    new CarQuoteRequestCarQuoteCarCoverInsurancePaymentType { insurancePaymentType = quoteDetails._insurancePaymentType, typeListName = "InsurancePaymentType_Ext" },
                previousPolicyExpire = DateTime.Now
            };
        }

        private CarQuoteRequestCarQuoteDrivers GetDriversDetails()
        {
            var drivers = new CarQuoteRequestCarQuoteDrivers { policyholder = GetPolicyHolderDeatils() };

            return drivers;
        }

        private CarQuoteRequestCarQuoteDriversPolicyholder GetPolicyHolderDeatils()
        {
            primaryPhone phone = primaryPhone.home;
            if (quoteDetails._primaryPhone.Equals("mobile"))
            {
                phone = primaryPhone.mobile;
            }
            else if (quoteDetails._primaryPhone.Equals("work"))
            {
                phone = primaryPhone.work;
            }

            var policyHolder = new CarQuoteRequestCarQuoteDriversPolicyholder
            {
                name = new nameType
                {
                    fullName =
                        new nameTypeFullName
                        {
                            firstName = quoteDetails._forename,
                            lastName = quoteDetails._surname,
                            title = new nameTypeFullNameTitle { title = "003_Mr", typeListName = "NamePrefix" },
                            titleDesc = "title values"
                        }
                },
                driverId = "1",
                dateOfBirth = Convert.ToDateTime(quoteDetails._dateOfBirthAsString),
                maritalStatus = new CarQuoteRequestCarQuoteDriversPolicyholderMaritalStatus
                {
                    maritalStatus = "B",
                    typeListName = "MaritalStatus"
                },
                maritalStatusDesc = "marital status values",
                address = GetAddressDetails(),
                ukResident = Convert.ToDateTime(quoteDetails._dateOfBirthAsString),
                marketingPreference = GetMarketingPreferenceDetails(),
                employment = GetEmploymentDetails(),
                licence = GetLicenceDetails(),
                myLicenceIndicator = false,
                useOtherVehicle =
                    new CarQuoteRequestCarQuoteDriversPolicyholderUseOtherVehicle { useOtherVehicle = "own_van", typeListName = "AccessOtherVeh_Ext" },
                useOtherVehicleDesc = "useother values",
                primaryEmail = quoteDetails._primaryEmail,
                secondaryEmail = quoteDetails._secondaryEmail,
                homePhoneNumber = quoteDetails._homePhoneNumber,
                workPhoneNumber = quoteDetails._workPhoneNumber,
                mobilePhoneNumber = quoteDetails._mobilePhoneNumber,
                faxPhoneNumber = quoteDetails._faxPhoneNumber,
                primaryPhone = phone,
                medicalConditions = new CarQuoteRequestCarQuoteDriversPolicyholderMedicalConditions
                {
                    medicalConditions = "99_NO",
                    typeListName = "DVLAMedicalCondition_Ext"
                },
                medicalConditionsDesc = "medical condition values",
                nonMotoringConvictions = quoteDetails._nonMotoringConvictions,
                isPolicyDeclinedForDrivers = quoteDetails._isPolicyDeclinedForDrivers,
                regularlyDriveAtPeakTimes = quoteDetails._regularlyDriveAtPeakTimes
            };

            return policyHolder;
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

        private CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreference GetMarketingPreferenceDetails()//was static
        {
            var fieldsData = new CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreferenceFieldsData[1];
            fieldsData[0] = new CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreferenceFieldsData
            {
                code = "test",
                value = "test"
            };

            return new CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreference
            {
                homeOwner = true,
                homeInsuranceRenewal =
                    new CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreferenceHomeInsuranceRenewal
                    {
                        homeInsuranceRenewal = "march",
                        typeListName = "InsuranceRenewalMonth_Ext"
                    },
                timeAtCurrentAddress =
                    new CarQuoteRequestCarQuoteDriversPolicyholderMarketingPreferenceTimeAtCurrentAddress
                    {
                        timeAtCurrentAddress = quoteDetails._timeAtCurrentAddress,
                        typeListName = "NCDGrantedYears_Ext"
                    },
                anyChildrenUnder16 = quoteDetails._anyChildrenUnder16,
                fieldsData = fieldsData
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

        private licenceType GetLicenceDetails()
        {
            return new licenceType
            {
                number = quoteDetails._drivingLicenceNo,
                type = new licenceTypeType { type = "F_FM", typeListName = "LicenceType_Ext" },
                typeDesc = "licencetype values",
                lengthHeld = Convert.ToDateTime(quoteDetails._drivingLicenceHeldSinceAsString),
                fullUkLicenceIamCert = false,
                yearPassPlusCert = false
            };
        }

        private static CarQuoteRequestCarQuoteExtraQuestions GetExtraQuestionsDetails()
        {
            return new CarQuoteRequestCarQuoteExtraQuestions
            {
                vanInsured = false,
                renewalVanDue = DateTime.Today,
                bikeInsured = false,
                renewalBikeDue = DateTime.Today,
                homeInsured = false,
                renewalPrice = "204",
                bestPrice = "304"
            };
        }
    }
}
