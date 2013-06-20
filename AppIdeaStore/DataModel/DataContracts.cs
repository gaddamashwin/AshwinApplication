using AppIdeaStore.ServiceRef;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace AppIdeaStore.DataModel
{
    [Table("DcSectorData")]
    public class DcSectorData : DcSector
    {
        [PrimaryKey, AutoIncrement]
        public Int32 PkId { get; set; }
    }

    [Table("DcAppDetails2Data")]
    public class DcAppDetails2Data : DcAppDetails2
    {
        [PrimaryKey, AutoIncrement]
        public Int32 PkId { get; set; }
    }

    [Table("DcAppData")]
    public class DcAppData : DcApp
    {
        [PrimaryKey, AutoIncrement]
        public Int32 PkId { get; set; }

        public BitmapImage MyImage
        {
            get
            {
                return Helper.ByteArrayToImage(pict);
            }
        }

    }

    [Table("DcAppDetailData")]
    public class DcAppDetailData : DcAppDetail
    {
        [PrimaryKey, AutoIncrement]
        public Int32 PkId { get; set; }

        public BitmapImage MyImage
        {
            get
            {
                return Helper.ByteArrayToImage(pict);
            }
        }

    }

    public class AppData : DcAppData
    {
        private IEnumerable<DcAppDetail> _AppDetails;
        private IEnumerable<DcAppDetails2Data> _AppDetails2;

        public IEnumerable<DcAppDetail> AppDetails
        {
            get
            {
                //foreach (var item in DataCollection.ListAppDetails.Where(i => i.appId.Equals(base.appId))) _AppDetails.Add(item); return _AppDetails; 
                if (_AppDetails == null)
                    _AppDetails = from appdetails in DataCollection.ListAppDetails
                                  where appdetails.appId.Equals(base.appId)
                                  select appdetails;

                return _AppDetails;
            }
        }
        public IEnumerable<DcAppDetails2Data> AppDetails2
        {
            get
            {
                //foreach (var item in DataCollection.ListApp2Details.Where(i => i.appId.Equals(base.appId)))_AppDetails2.Add(item); return _AppDetails2; 
                if (_AppDetails2 == null)
                    _AppDetails2 = from appdetails in DataCollection.ListApp2Details
                                   where appdetails.appId.Equals(base.appId)
                                   select appdetails;

                return _AppDetails2;
            }
        }
    }

    public class SectorAppData : DcSector
    {
        private IEnumerable<DcApp> _AppItems;

        public IEnumerable<DcApp> AppItems
        {
            get
            {
                if (_AppItems == null)
                    _AppItems = from sec in DataCollection.ListAppSectors
                                join app in DataCollection.ListApps on sec.appId equals app.appId
                                where sec.sectorId == base.sectorId
                                select app;
                return _AppItems;
            }
        }

        public IEnumerable<DcApp> TopAppItems
        {
            get { return this.AppItems.Take(8); }
        }

        public string count
        {
            get;
            set;
        }
    }
}
