using GeoCoordinatePortable;
using IPGeoFencing.GeographicAreas.DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IPGeoFencing.UnitTests.GeographicAreas
{
    [TestClass]
    public class BaseGeographicAreaTest
    {
        protected IEnumerable<GeoCoordinate> GetTestTriangleCoordinates(double x)
        {
            return new GeoCoordinate[] {
                new GeoCoordinate(0, x),
                new GeoCoordinate(-x, 0),
                new GeoCoordinate(x, 0)
            };
        }

        protected CircleAreaModel GetTestCircleArea(double radius, GeoCoordinate? center = null)
        {
            if (center is null)
                center = new GeoCoordinate(0, 0);

            return new CircleAreaModel("circle", center, radius);
        }

        protected PerimeterModel GetTestSquarePermeter(double x, double offset = 0)
        {
            var coordinates = new GeoCoordinate[] {
                new GeoCoordinate(-x+offset, -x+offset),
                new GeoCoordinate(-x+offset, x+offset),
                new GeoCoordinate(x+offset, x+offset),
                new GeoCoordinate(x+offset, -x+offset)                               
            };

            return new PerimeterModel(coordinates);
        }
    }
}