﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppIdeaStore.Common;
using AppIdeaStore.ServiceRef;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Collections;
using System.Reflection;
using SQLite;

namespace AppIdeaStore.DataModel
{
    public class Helper
    {
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
        public static T CopyProperties<T>(object src) where T:new()
        {
            var dst = new T();
            IEnumerable<PropertyInfo> srcProperties = src.GetType().GetRuntimeProperties();
            dynamic dstType = dst.GetType();

            if (srcProperties == null | dstType.GetProperties() == null)
            {
                return dst;
            }

            foreach (PropertyInfo srcProperty in srcProperties)
            {
                PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

                if (dstProperty != null)
                {
                    if (dstProperty.CanWrite == true)
                    {
                        dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
                    }
                }
            }
            return dst;
        }
        private static ServiceRef.Service1Client svc;
        public static ServiceRef.Service1Client appService
        {
            get
            {
                if (svc == null)
                {
                    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    //svc = new ServiceRef.Service2Client(binding, new System.ServiceModel.EndpointAddress("http://USCMPUJMITTAL8.us.deloitte.com/WcfService/Service2.svc"));10.9.183.121
                    svc = new ServiceRef.Service1Client(binding, new System.ServiceModel.EndpointAddress("http://USCMPUJMITTAL8.us.deloitte.com/WcfService/Service1.svc"));
                    //if (svc.State != System.ServiceModel.CommunicationState.Opened)
                    //{ 
                    //    string a = "a";
                    //}
                }
                return svc;
            }
        }
        private static SQLiteAsyncConnection _db;
        public static SQLiteAsyncConnection db
        {
            get
            {
                if (_db == null)
                {
                    var path = Windows.Storage.ApplicationData.Current.LocalFolder.Path + @"\AppDataDemo.db";
                    _db = new SQLiteAsyncConnection(path);
                    var data = _db.QueryAsync<DcAppDetailData>("SELECT name FROM sqlite_master WHERE type='table';").Result;
                    if (data.Count <= 4)
                    {
                        deleteTables();
                        var resut =  _db.CreateTablesAsync(new Type[] { typeof(DcSectorData), typeof(DcAppDetails2Data), typeof(DcAppData), typeof(DcAppDetailData) });
                        while (resut.Status != TaskStatus.RanToCompletion) { }
                        //DcSectorData
                        //_db.CreateTableAsync<DcSectorData>();
                        //_db.CreateTableAsync<DcAppDetails2Data>();
                        //_db.CreateTableAsync<DcAppData>();
                        //_db.CreateTableAsync<DcAppDetailData>();
                    }

                    //while (_db.QueryAsync<DcAppDetailData>("SELECT name FROM sqlite_master WHERE type='table';").Result.Count < 4) { }

                    //await db.CreateTableAsync<DcSectorData>();
                    //await db.CreateTableAsync<DcAppDetails2Data>();
                    //await db.CreateTableAsync<DcAppData>();
                    //await db.CreateTableAsync<DcAppDetailData>();
                }
                return _db;
            }
        }

        public static async void deleteTables()
        {
            var r1 = await db.DropTableAsync<DcSectorData>();
            var r2 = await db.DropTableAsync<DcAppDetails2Data>();
            var r3 = await db.DropTableAsync<DcAppData>();
            var r4 = await db.DropTableAsync<DcAppDetailData>();

            //while (r1.Status != TaskStatus.RanToCompletion &&
            //    r2.Status != TaskStatus.RanToCompletion &&
            //    r3.Status != TaskStatus.RanToCompletion &&
            //    r4.Status != TaskStatus.RanToCompletion) { }
        }

        public static void ClearData()
        {
            deleteTables();
            _db = null;
            DataCollection.clearCollection();
        }
    }

    public class DataCollection
    {
        public static Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static List<SectorAppData> _sectorGroups;
        private static IEnumerable<DcSectorData> _ListAppSectors;
        private static IEnumerable<DcAppDetails2Data> _ListApp2Details;
        private static IEnumerable<DcAppDetailData> _ListAppDetails;
        private static IEnumerable<DcAppData> _ListApps;

        public static IEnumerable<DcAppData> ListApps
        {
            get
            {
                if (_ListApps == null)
                {
                    _ListApps = Helper.db.QueryAsync<DcAppData>("SELECT * FROM DcAppData").Result;
                    if (!_ListApps.Any()) 
                    { 
                        _ListApps = Helper.appService.GetAppIdeaAsync().Result.Select(i => Helper.CopyProperties<DcAppData>(i));
                        Helper.db.InsertAllAsync(_ListApps);
                    }
                }
                return _ListApps;
            }
        }
        public static IEnumerable<DcAppDetailData> ListAppDetails
        {
            get
            {
                if (_ListAppDetails == null)
                {
                    _ListAppDetails = Helper.db.QueryAsync<DcAppDetailData>("SELECT * FROM DcAppDetailData").Result;
                    if (!_ListAppDetails.Any())
                    {
                        _ListAppDetails = Helper.appService.GetAppDetailAsync().Result.Select(i => Helper.CopyProperties<DcAppDetailData>(i));
                        Helper.db.InsertAllAsync(_ListAppDetails);
                    }
                }
                return _ListAppDetails;
            }
        }
        public static IEnumerable<DcAppDetails2Data> ListApp2Details
        {
            get
            {
                if (_ListApp2Details == null)
                {
                    _ListApp2Details = Helper.db.QueryAsync<DcAppDetails2Data>("SELECT * FROM DcAppDetails2Data").Result;
                    if (!_ListApp2Details.Any())
                    {
                        _ListApp2Details = Helper.appService.GetAppDetails2Async().Result.Select(i => Helper.CopyProperties<DcAppDetails2Data>(i));
                        Helper.db.InsertAllAsync(_ListApp2Details);
                    }
                }
                return _ListApp2Details;
            }
        }
        public static IEnumerable<DcSectorData> ListAppSectors
        {
            get
            {
                if (_ListAppSectors == null)
                {
                    _ListAppSectors = Helper.db.QueryAsync<DcSectorData>("SELECT * FROM DcSectorData").Result;
                    if (!_ListAppSectors.Any())
                    {
                        var r = Helper.appService.GetAppSectorAsync().Result;
                        _ListAppSectors = r.Select(i => Helper.CopyProperties<DcSectorData>(i));
                        Helper.db.InsertAllAsync(_ListAppSectors);
                    }
                }
                return _ListAppSectors;
            }
        }

        public static void clearCollection()
        {
            if (_sectorGroups != null) { _sectorGroups.Clear(); _sectorGroups = null; }
            if (_ListAppSectors != null) { _ListAppSectors.ToList().Clear(); _ListAppSectors = null; }
            if (_ListApp2Details != null) { _ListApp2Details.ToList().Clear(); _ListApp2Details = null; }
            if (_ListAppDetails != null) { _ListAppDetails.ToList().Clear(); _ListAppDetails = null; }
            if (_ListApps != null) { _ListApps.ToList().Clear(); _ListApps = null; }
        }
    }

    public class AppIdeaDataSource
    {
        public static IEnumerable<SectorAppData> GetSectorGroups()
        {
            if (DataCollection._sectorGroups == null || DataCollection._sectorGroups.Count() == 0)
            {
                DataCollection._sectorGroups = new List<SectorAppData>();
                foreach (var item in DataCollection.ListAppSectors.GroupBy(i => i.sectorId))
                {
                    SectorAppData sector = Helper.CopyProperties<SectorAppData>(DataCollection.ListAppSectors.Where(i => i.sectorId == item.Key).FirstOrDefault());
                    sector.count = item.Count().ToString();
                    DataCollection._sectorGroups.Add(sector);
                }
            }
            return DataCollection._sectorGroups;
        }

        public static SectorAppData GetGroup(int sectorID)
        {
            return DataCollection._sectorGroups.Where(i => i.sectorId == sectorID).FirstOrDefault();
        }

        public static AppData GetAppData(int AppID)
        {
            return DataCollection.ListApps.Where(i => i.appId == AppID).Select(i => Helper.CopyProperties<AppData>(i)).FirstOrDefault();
        }
    }
}
