using System.Linq;
using Core.Algorithm;
using Xunit;

namespace Core.Test
{
    public class MeshBuilderTest
    {
        [Fact]
        public void InsidePolygon()
        {
            var lst = new[]
            {
                new LoopBuilder().AddRectangle(2, 2, 1, 1).Build().ToArray()
            };
            Assert.True(MeshBuilder.InsidePolygons(lst, new Point2(2, 2)));
            Assert.True(MeshBuilder.InsidePolygons(lst, new Point2(1, 2)));
            Assert.False(MeshBuilder.InsidePolygons(lst, new Point2(0.1f, 2)));
        }
    }
}
