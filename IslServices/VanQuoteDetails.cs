using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslServices
{
    public class VanQuoteDetails
    {
        public string _vanRegistration { get; set; } = "YD61GVZ";
        public string _noOfDriversinHouse { get; set; } = "1";
        public string _noOfVehiclesHousehold { get; set; } = "1";
        public DateTime _inceptionDate { get; set; } = DateTime.Today;
        public DateTime _purchaseDate { get; set; } = DateTime.Today;
        public int _VanValue { get; set; } = 10000;
        public string _postcode { get; set; } = "DN45EJ";
        public string _businessMileage { get; set; } = "0";
        public string _personalMileage { get; set; } = "10000";
        public string _totalMileage { get; set; } = "10000";
        public string _ncdGrantedYears { get; set; } = "5";
        public string _primaryEmail { get; set; } = "test@hastingdirect.com";
        public string _secondaryEmail { get; set; } = "test@hastingdirect.com";
        public string _homePhoneNumber { get; set; } = "01424735735";
        public string _workPhoneNumber { get; set; } = "01424735735";
        public string _mobilePhoneNumber { get; set; } = "01424735735";
        public string _faxPhoneNumber { get; set; } = "01424735735";
        public string _primaryPhone { get; set; } = "home";
        public bool _homeOwner { get; set; } = true;
        public bool _anyChildrenUnder16 { get; set; } = false;
        public bool _nonMotoringConvictions { get; set; } = false;
        public bool _isPolicyDeclinedForDrivers { get; set; } = false;
        public bool _regularlyDriveAtPeakTimes { get; set; } = true;
        public string _timeAtCurrentAddress { get; set; } = "4";
        public string _aggSource { get; set; } = "uSwitch";
        public string _forename { get; set; } = "Dan";
        public string _surname { get; set; } = "Cooper";
        public string _dateOfBirthAsString { get; set; } = "01/01/1990";
        public string _addressFirstLine { get; set; } = "10 St Hildas Road";
        public string _drivingLicenceNo { get; set; } = "";
        public string _drivingLicenceHeldSinceAsString { get; set; } = "01/01/2009";
    }
}
