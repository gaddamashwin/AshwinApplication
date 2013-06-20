using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Common;

namespace TestApp.DataModel
{
    public class SectorAppGroup : BindableBase
    {

        public SectorAppGroup()
        {
        }

        public string Title { get; set; }
        public string IdType { get; set; }

        public SectorAppGroup(string name, string Id)
        {
            this.Title = name;
            this.IdType = Id;
        }

        private ObservableCollection<appItem> _items = new ObservableCollection<appItem>();
        public ObservableCollection<appItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<appItem> TopItems
        {
            get { return this._items.Take(8); }
        }

    }
    public class appItem : BindableBase
    {
        public appItem(string n, string id)
        {
            Title = n;
            AppIdType = id;
        }

        public string Title { get; set; }
        public string AppIdType { get; set; }

        private SectorAppGroup _group;
        public SectorAppGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public sealed class AppDataSource
    {
        private ObservableCollection<SectorAppGroup> _allGroups = new ObservableCollection<SectorAppGroup>();
        public ObservableCollection<SectorAppGroup> AllGroups
        {
            get { return _allGroups; }
        }

        public IEnumerable<SectorAppGroup> GetGroups()
        {
            var g1 = new SectorAppGroup("ashwin", "ashwin");
            g1.Items.Add(new appItem("item1","as"));

            AllGroups.Add(g1);
            return AllGroups;
        }
    }
}
