//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public string comment1 { get; set; }
        public int commentId { get; set; }
        public int appId { get; set; }
        public int userId { get; set; }
        public int rating { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> date { get; set; }
    
        public virtual App App { get; set; }
        public virtual User User { get; set; }
    }
}
