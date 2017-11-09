using DotSpatial.Positioning;

namespace location
{
    public class LatLong
    {
        public double Lat { get; set; }
        public double Long { get; set; }

        public Position Position =>
            new Position(new Latitude(Lat), new Longitude(Long));
    }
    
    public class PlacesConfig
    {
        public string LocationsFile { get; set; }
        
        public LatLong Home { get; set; }
        public LatLong Work { get; set; }
        
        public string FromCmd { get; set; }
    }
}