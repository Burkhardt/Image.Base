// $Header: http://svn.jgency.net:9880/JgenCy/Solutions/HDitem/HDitem.Image.Types/ImageSizes.cs 619 2012-06-04 18:09:06Z RSB $

// Requirements
// A class ImagesSizes is needed that stores the dimension {Width, Height} of an image on disk.
// An object of this class will be stored in the Application object; threadsafe updates/bulk inserts
// must be possible within a Application.Lock/Unlock bracket;
// this cache must expire which means, that the age of the value always has to be checked in the Contains() method

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using RaiImage;

namespace RaiImage
{
    public class ImageTypes
    {
        public static char[] Seperator = new char[] { ',', ';', ' ', '\t' };
        public static ImageTypes Default = new ImageTypes(new string[] { "tiff", "png", "jpg", "psd" });
        private string[] ext;
        public string[] Array
        {
            get { return ext; }
            set { ext = value; }
        }
        public string String
        {
            get
            {
                string extensions = "";
                foreach (string s in ext)
                    extensions += extensions.Length > 0 ? ", " + s : s;
                return extensions;
            }
            set { ext = value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries); }
        }
        public ImageTypes(string extensions)
        {
            String = extensions;
        }
        public ImageTypes(string[] extensions)
        {
            Array = extensions;
        }
    }
    public class Pane
    {
        public static Pane DefaultPane = new Pane("160x200");
        private string wxh;
        public string String
        {
            get { return wxh; }
        }
        public System.Drawing.Size Size
        {
            get
            {
                System.Drawing.Size size = new System.Drawing.Size(0, 0);
                if (!string.IsNullOrEmpty(wxh) && wxh.Length >= 3 && wxh.IndexOf('x') >= 0)
                {
                    int i = wxh.IndexOf('x');
                    size.Width = int.Parse(wxh.Substring(0, i));
                    size.Height = int.Parse(wxh.Substring(i + 1));
                }
                return size;
            }
            set
            {
                wxh = value.Width.ToString() + "x" + value.Height.ToString();
            }
        }
        public Pane(string WxH)
        {
            wxh = WxH;
        }
        public Pane(int width, int height)
        {
            wxh = width.ToString() + "x" + height.ToString();
        }
    }
    public class Panes
    {
        private Pane[] p;
        private string s;
        public int Count
        {
            get { return p.Length; }
        }
        public string String
        {
            get
            {
                return s;
            }
        }
        public Pane this[int i]
        {
            get
            {
                return p[i];
            }
        }
        /// <summary>
        /// Panes the viewer defines; zoomPortPane and ControlPortPane
        /// </summary>
        public Pane ZoomPort
        {
            get { return p.Length > 0 ? this[0] : Pane.DefaultPane; }
        }
        /// <summary>
        /// Panes the viewer defines; zoomPortPane and ControlPortPane
        /// </summary>
        public Pane ControlPort
        {
            get { return p.Length > 1 ? this[1] : ZoomPort; }
        }
        /// <summary>
        /// Constructor from formatted string ShowTrees' Pane Size in [0] (zooming), ControlPort in [1]
        /// </summary>
        /// <param name="panes">i.e. 352x440,320x400 or 352x440</param>
        public Panes(string panes)
        {
            s = panes;
            string[] sizes = panes.Split(new char[] { ',' }, StringSplitOptions.None);
            List<Pane> viewPorts = new List<Pane>();
            foreach (string pane in sizes)
                viewPorts.Add(new Pane(pane));
            p = viewPorts.ToArray();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoomWidth"></param>
        /// <param name="zoomHeight"></param>
        /// <param name="controlWidth"></param>
        /// <param name="controlHeight"></param>
        public Panes(int zoomWidth, int zoomHeight, int controlWidth, int controlHeight)
        {
            List<Pane> pList = new List<Pane>(2);
            pList.Add(new Pane(zoomWidth, zoomHeight));
            pList.Add(new Pane(controlWidth, controlHeight));
            p = pList.ToArray();
        }
    }
    /// <summary>
    /// this class is a lightweight solution for seperating the parameters of a src parameter as used in several places, i.e. the URL for HDitem.aspx
    /// </summary>
    public class Src
    {
        private string src;
        private ImageFile image;
        public bool HasMultipleSkus
        {
            get
            {
                return src.Contains(" ") || src.Contains("%20") || src.Count(f => f == '/') > 1;
            }
        }
        public string[] Skus
        {
            get
            {
                var s = src.Substring(src.IndexOf('/') + 1).Replace("%20", ",").Replace(" ", ",");
                return s.Split(new char[] { ',' });
            }
        }
        public string Sku
        {
            get
            {
                if (image == null)
                    image = new ImageFile(src);
                return image.Sku;
            }
        }
        public string Subscriber
        {
            get
            {
                if (image == null)
                    image = new ImageFile(src);
                return image.Path.Substring(0, image.Path.Length - 1);
            }
        }
        public int ImageNumber
        {
            get
            {
                if (image == null)
                    image = new ImageFile(src);
                return image.ImageNumber;
            }
        }
        public string Image
        {
            get
            {
                if (image == null)
                    image = new ImageFile(src);
                return image.Name;
            }
        }
        //public string ImageWithExtension
        //{
        //	get
        //	{
        //		if (image == null)
        //			image = new ImageFile(src);
        //		return image.NameWithExtension;
        //	}
        //}

        public string String
        {
            get { return src; }
        }
        public string Param()
        {
            return "src=" + this.String;
        }
        public Src(string src)
        {
            this.src = src.Replace("%2F", "/");
            image = null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Tmp
    {
        //private string tmp;
        public string Template
        {
            get
            {
                return template;
            }
            set
            {
                var a = value.Split(new char[] { '=' });
                var val = a.Last();
                if (string.IsNullOrEmpty(val))
                {
                    template = val;
                    return;
                }
                var array = val.CamelSplit();
                template = array[0];
                if (array.Length > 1)
                    Overlays = array.Skip(1).ToList();
            }
        }
        private string template;
        public List<string> Overlays
        {
            get
            {
                return overlays;
            }
            set
            {
                if (value == null || value.Count() == 0)
                    overlays = new List<string>();
                foreach (var s in value)
                    overlays.Add(s.ToTitle());
            }
        }
        private List<string> overlays = new List<string>();
        public string String
        {
            get
            {
                return Template + string.Join("", Overlays);
            }
        }
        public string Param()
        {
            return "tmp=" + this.String;
        }
        public Tmp(string tmpString)
        {
            Template = tmpString;
        }
    }
    public class IservUrl
    {
        protected UriBuilder u { get; set; }
        public string Subscriber { get; protected set; }
        public string Protocol => u.Scheme;
        public string Host => u.Host;
        public int Port => u.Port;
        /// <summary>
        /// i.e. /iserv/Office/Login.aspx or iserv/Office/Login.aspx or /iserv/Office/ or iserv/Office/
        /// </summary>
        public string Path => u.Path;
        public string App // i.e. iserv
        {
            get
            {
                var segments = u.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                return segments.Length > 0 ? segments[0] : "";
            }
            set
            {
                var oldApp = u.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                u.Path = u.Path.Replace(oldApp + "/", value + "/");
            }
        }
        public string Page // i.e. Version.aspx
        {
            get
            {
                var r = u.Path.LastIndexOf('/');
                return u.Path.Substring(r).Contains('.') ? u.Path.Substring(r + 1) : "";
            }
            set
            {
                var r = u.Path.LastIndexOf('/');
                if (u.Path.Substring(r).Contains('.'))
                    u.Path = u.Path.Substring(0, r) + value;
                else u.Path = u.Path.EndsWith("/") ? u.Path + value : u.Path + "/" + value;
            }
        } // private string page;
        public Uri Uri
        {
            get
            {
                return u.Uri;
            }
        }
        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="uri">new Uri(new Uri("http://pic.hse24.de/"), uri) to make sure that paths can be extended to the given base Urls context fot gilling in thr midding parts</param>
        protected void init(Uri uri)
        {
            u = new UriBuilder(uri);
            #region error if relative and no context
            // throws UriFormatException if Uri is "/picture/HDitem.aspx?src=pic/382912_01&tmp=Hugemobileshop", but not for "http://pic.hse24.de/picture/HDitem.aspx?src=pic/382912_01&tmp=Hugemobileshop"
            // looks like the Uri constructor is using a "context" to fill-in the Uri components not available in the string value;
            // works for running webApp (assumed that the investigated Url came in as a request with the same context) but not for console app (no context)
            #endregion
            // if (!uri.IsAbsoluteUri && HttpContext.Current == null)
            // 	u = new UriBuilder(uri);    // this creates a fake context to fill-in the missing parts; it sort of works but might create false records for host, port, protocol, ...
        }
        public IservUrl(string uri)
        {
            init(new Uri(uri, UriKind.RelativeOrAbsolute));
        }
        public IservUrl(Uri uri)
        {
            init(uri);
        }
    }
    public class ServiceUrl : IservUrl
    {
        public void init(Uri uri, bool callBaseInit = true)
        {
        }
        public ServiceUrl(Uri uri)
            : base(uri)
        {
            init(uri, false);   // call only the local init for ImageUrl; IservUrl.init was called already
        }
    }
    /// <summary>
    /// Breaking down everything a valid HDitem.aspx call contains
    /// </summary>
    public class ImageUrl : IservUrl
    {
        public Src Src { get; set; }
        public Tmp Tmp { get; set; }
        public bool isHDitemLink()
        {
            return Tmp != null && !string.IsNullOrEmpty(Src.Subscriber);
        }
        public void init(Uri uri, bool callBaseInit = true)
        {
            if (callBaseInit)
                init(uri);
            if (!string.IsNullOrEmpty(u.Query))
            {
                var p = HttpUtility.ParseQueryString(u.Query);
                Src = new Src(p["src"]);
                Subscriber = Src.Subscriber;
                Tmp = p["tmp"] == null ? null : new Tmp(p["tmp"]);
            }
            #region try to get the SKU for non-HDitem links - indicates start of imageNumber; i.e. forzieri
            if (Src == null || string.IsNullOrEmpty(Src.Sku))
            {
                var regex = new Regex(@"([a-z]{2}\d{6}-\d{3}-\d{2})-(\d{1})x");     // get it from Shop
                var img = new ImageFile(Src.Image);
                if (img.ImageNumber < 0)
                {
                    var match = regex.Match(img.Name);
                    if (match.Success && match.Groups.Count >= 3)
                    {
                        img.Name = match.Groups[1].Value;
                        img.ImageNumber = int.Parse(match.Groups[2].Value);
                    }
                    // What I am doing with it? write to Sku?
                    // Where do i save the information how to reconstruct the original Sku with image?
                }
            }
            #endregion
        }
        //public string Url
        //{
        //	get
        //	{
        //		var s = "";
        //		if (!string.IsNullOrEmpty(Protocol))
        //			s += Protocol + "://";
        //		if (!string.IsNullOrEmpty(Host))
        //			s += Host;
        //		if (Port != 0 && Port != 80)
        //			s += Port.ToString();
        //		if (!string.IsNullOrEmpty(Path))
        //			s += Path;
        //		if (Tmp == null && Src != null)
        //			return s + "/" + Src.Image;	// traditional image link without subscriber and template
        //		return s + "?" + Src.Param() + "&" + Tmp.Param();
        //	}
        //	set
        //	{
        //		init(new Uri(new Uri("http://pic.hse24.de/"), value));
        //		if (!string.IsNullOrEmpty(u.Query))
        //		{
        //			var p = HttpUtility.ParseQueryString(u.Query);
        //			Src = new Src(p["src"]);
        //			Subscriber = Src.Subscriber;
        //			Tmp = p["tmp"] == null ? null : new Tmp(p["tmp"]);
        //		}
        //		#region try to get the SKU for non-HDitem links - indicates start of imageNumber; i.e. forzieri
        //		if (Src == null || string.IsNullOrEmpty(Src.Sku))
        //		{
        //			var regex = new Regex(@"([a-z]{2}\d{6}-\d{3}-\d{2})-(\d{1})x");     // get it from Shop
        //			var img = new ImageFile(Src.Image);
        //			if (img.ImageNumber < 0)
        //			{
        //				var match = regex.Match(img.Name);
        //				if (match.Success && match.Groups.Count >= 3)
        //				{
        //					img.Name = match.Groups[1].Value;
        //					img.ImageNumber = int.Parse(match.Groups[2].Value);
        //				}
        //				// What I am doing with it? write to Sku?
        //				// Where do i save the information how to reconstruct the original Sku with image?
        //			}
        //		}
        //		#endregion
        //	}
        //}
        //public ImageUrl(string imageUrl)
        //	: base(imageUrl)
        //{
        //	init(imageUrl);  // call only the local init for ImageUrl; IservUrl.init was called already
        //}
        /// <summary>
        /// constructor - make sure uri is either contructed in an active context or absolute or a baseUrl is added as context
        /// </summary>
        /// <param name="uri">new Uri(new Uri(baseUrlString), relativeUrlString)</param>
        public ImageUrl(Uri uri)
            : base(uri)
        {
            init(uri, false);   // call only the local init for ImageUrl; IservUrl.init was called already
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TwoSizes : IComparable<TwoSizes>
    {
        private float rating;
        public float Rating
        {
            get { return Math.Abs(rating) < float.Epsilon ? 0F : rating; }
            set { rating = value; }
        }
        internal System.Drawing.Size smallRect;
        public System.Drawing.Size SmallRect
        {
            get { return smallRect; }
        }
        internal System.Drawing.Size largeRect;
        public System.Drawing.Size LargeRect
        {
            get { return largeRect; }
        }
        public int CompareTo(TwoSizes other)
        {
            if (other == null)
                return 1;
            float f = Rating - other.Rating;
            return f < 0 ? -1 : f > 0 ? 1 : 0;
        }
        public bool Equals(TwoSizes other)
        {
            return SmallRect.Width == other.SmallRect.Width && SmallRect.Height == other.SmallRect.Height;
        }
        public TwoSizes(int smallW = 0, int smallH = 0, int largeW = 0, int largeH = 0, float rating = 0F)
        {
            smallRect = new System.Drawing.Size(smallW, smallH);
            largeRect = new System.Drawing.Size(largeW, largeH);
            this.rating = rating;
        }
    }
}
