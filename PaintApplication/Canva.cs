//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PaintApplication
{
    using System;
    using System.Collections.Generic;
    
    public partial class Canva
    {
        public int Id { get; set; }
        public byte[] InkStrokes { get; set; }
        public byte[] UserPhoto { get; set; }
        public Nullable<System.DateTimeOffset> CreatedUtc { get; set; }
    }
}