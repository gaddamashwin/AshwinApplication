using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshwinApplication.Common;
using AshwinApplication.ServiceRef;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Collections;

namespace AshwinApplication.DataModel
{
    public class Helper
    {
        public static string WriteXML<T>(T saveObject)
        {
            //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            //StringWriter str = new StringWriter();
            //writer.Serialize(str, saveObject);
            DataContractSerializer writer = new DataContractSerializer(typeof(T));
            MemoryStream str = new MemoryStream();
            writer.WriteObject(str, saveObject);
            return "";
        }

        public static T ReadXML<T>(object str)
        {
            //System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
            //StringReader strR = new StringReader(str);
            //return (T)reader.Deserialize(strR); 

            DataContractSerializer reader = new DataContractSerializer(typeof(T));
            MemoryStream strR = (MemoryStream)str;
            return (T)reader.ReadObject(strR);
        }

        public static BitmapImage ByteArrayToImage(byte[] pict)
        {
            if (pict != null)
            {
                BitmapImage image1 = new BitmapImage();
                InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
                ms.WriteAsync(pict.AsBuffer()).GetResults();
                ms.FlushAsync().AsTask().Wait();
                ms.Seek(0);
                image1.SetSource(ms);
                return image1;
            }
            else return null;
        }

        private static ServiceRef.Service1Client svc;
        public static ServiceRef.Service1Client appService { 
            get 
            {
                if (svc == null)
                {
                    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    svc = new ServiceRef.Service1Client(binding, new System.ServiceModel.EndpointAddress("http://USCMPUJMITTAL8.us.deloitte.com/WcfService/Service1.svc"));
                }
                return svc;
            } 
        }
    }

    [DataContract]
    public class AppData 
    {
        [DataMember]
        public byte[] Pict;

        [DataMember]
        public int appId;

        [DataMember]
        public bool deleted;

        [DataMember]
        public string desc;

        [DataMember]
        public string idea;

        [DataMember]
        public string impact;

        [DataMember]
        public string issue;

        [DataMember]
        public string name;

        [DataMember]
        public int submittedBy;

        [DataMember]
        public System.DateTime submittedDate;

        public AppData(appType item)
        {
            this.appId = item.appId;
            this.deleted = Convert.ToBoolean(item.deleted);
            this.desc = item.desc;
            this.idea = item.idea;
            this.impact = item.impact;
            this.issue = item.issue;
            this.name = item.name;
            this.Pict = item.Pict;
            this.submittedBy = Convert.ToInt32(item.submittedBy);
            this.submittedDate = Convert.ToDateTime(item.submittedDate);
        }
        [DataMember]
        public BitmapImage MyImage
        {
            get
            {
                return Helper.ByteArrayToImage(Pict);
            }
        }

    }

    public class ImageData : ImageType
    {
        public ImageData(ImageType item)
        {
            base.Pict = item.Pict;
            base.AppId = item.AppId;
            base.ImageId = item.ImageId;
        }

        public BitmapImage MyImage
        {
            get
            {
                return Helper.ByteArrayToImage(Pict);
            }
        }

    }

    public class DataCollection
    {
        public static Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private static List<AppData> _appDataCollection = new List<AppData>();
        private static List<ImageData> _imageDataCollection = new List<ImageData>();
        private static ObservableCollection<SectorType> _sectorDataCollection = new ObservableCollection<SectorType>();
        private static ObservableCollection<SectorAppData> _allGroups = new ObservableCollection<SectorAppData>();
        public static ObservableCollection<SectorAppData> AllGroups
        {
            get
            {
                if (_allGroups != null && _allGroups.Count == 0)
                {
                    var sectors = Helper.appService.GetSectorAsync().Result;
                    foreach (var sector in sectors)
                    {
                        SectorAppData sectorApp = new SectorAppData(sector);
                        _allGroups.Add(sectorApp);
                    }
                }
                return _allGroups;
                //if (!localsettings.Containers.ContainsKey("AllGroups"))
                //{
                //    var sectors = Helper.appService.GetSectorAsync().Result;
                //    foreach (var sector in sectors)
                //    {
                //        SectorAppData sectorApp = new SectorAppData(sector);
                //        _allGroups.Add(sectorApp);
                //    }
                //    localsettings.Values["AllGroups"] = Helper.WriteXML<ObservableCollection<SectorAppData>>(_allGroups);
                //}
                //var rt = Helper.ReadXML<ObservableCollection<SectorAppData>>((string)localsettings.Values["AllGroups"]);
                //return rt;
            }
        }

        public static IEnumerable<AppData> appDataCollection 
        {
            get
            {
                if (_appDataCollection.Count == 0) foreach (var item in Helper.appService.GetAppAsync().Result) _appDataCollection.Add(new AppData(item));
                return _appDataCollection;
                //if (!localsettings.Containers.ContainsKey("appDataCollection")) { foreach (var item in Helper.appService.GetAppAsync().Result) _appDataCollection.Add(new AppData(item)); localsettings.Values["appDataCollection"] = _appDataCollection; }
                //return (IEnumerable<AppData>)localsettings.Values["appDataCollection"];
            }
        }

        public static IEnumerable<ImageData> imageDataCollection
        {
            get
            {
                //if (_imageDataCollection.Count == 0) foreach (var item in Helper.appService.GetImageAsync().Result) _imageDataCollection.Add(new ImageData(item));
                if (!localsettings.Containers.ContainsKey("imageDataCollection")) { foreach (var item in Helper.appService.GetImageAsync().Result) _imageDataCollection.Add(new ImageData(item)); localsettings.Values["imageDataCollection"] = _imageDataCollection; }
                return (IEnumerable<ImageData>)localsettings.Values["imageDataCollection"];
                //return _imageDataCollection;
            }
        }

        public static IEnumerable<SectorType> sectorDataCollection
        {
            get
            {
                if (_sectorDataCollection.Count == 0) _sectorDataCollection = Helper.appService.GetSectorAsync().Result;
                return _sectorDataCollection;
            }
        }
    }

    [CollectionDataContract]
    public class SectorAppData : List<object>
    //public class SectorAppData : BindableBase 
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int SectorId { get; set; }
        public SectorAppData() { }
        public SectorAppData(SectorType sector)
        {
            this.Name = sector.Name;
            this.SectorId = sector.SectorId;
            //IEnumerable<appType> result = Helper.appService.GetAppDetailsAsync(sector.SectorId, this.GetType().ToString()).Result;
            //foreach (appType item in result)
            //{
            //    _items.Add(new AppData(item));
            //}

            //var rt1 = (from app in DataCollection.appDataCollection where app.deleted == false select new { appId = app.appId, count = DataCollection.sectorDataCollection.Where(i => i.SectorId == SectorId).Count() }).ToList();
            var rt1 = (from app in DataCollection.sectorDataCollection where app.SectorId == SectorId group app by app.appID into distapp select new { appID = distapp.Key }).ToList();

            _items = (from app in DataCollection.appDataCollection
                      join dtl in rt1.SelectMany(i=>i.appID) on app.appId equals dtl
                      select app).ToList();
        }
        
        private List<AppData> _items = new List<AppData>();
        [DataMember]
        public IEnumerable<AppData> Items
        {
            get { return this._items; }
        }
        [DataMember]
        public IEnumerable<AppData> TopItems
        {
            get { return this._items.Take(8); }
        }
    }

    public class ImageDataGroup : BindableBase
    {
        public List<ImageData> _ImageDataGroups = new List<ImageData>();
        public IEnumerable<ImageData> ImagesData 
        {
            get 
            {
                if (_ImageDataGroups == null || _ImageDataGroups.Count() == 0)
                {
                    var sectors = Helper.appService.GetImageAsync().Result;
                    foreach (var sector in sectors)
                    {
                        ImageData sectorApp = new ImageData(sector);
                        _ImageDataGroups.Add(sectorApp);
                    }
                }

                return _ImageDataGroups;
            } 
        }
        public AppData appData { get; set; }
    }

    public sealed class AppDataSource
    {
       
        public static IEnumerable<SectorAppData> GetGroups()
        {

            return DataCollection.AllGroups;
        }

        public static SectorAppData GetGroup(int sectorID)
        {
            return DataCollection.AllGroups.Where(i => i.SectorId == sectorID).FirstOrDefault();
        }

        public static IEnumerable<ImageData> GetImageData(int appID)
        {
            return DataCollection.imageDataCollection.Where(i => i.AppId == appID);
        }

        public static AppData GetAppData(int appID)
        {
            return DataCollection.appDataCollection.Where(i => i.appId == appID).FirstOrDefault();
        }

    }
}
