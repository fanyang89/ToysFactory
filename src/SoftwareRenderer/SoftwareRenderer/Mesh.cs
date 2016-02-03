using System.Collections.Generic;
using FadeJson;

namespace SoftwareRenderer
{
    public class Mesh
    {
        //Hide default ctor
        private Mesh() { }

        public Vector Position { get; set; } = Vector.Zero;
        public Vector Rotation { get; set; } = Vector.Zero;
        public Surface[] Surfaces { get; private set; }
        public Vector[] Vertices { get; private set; }

        public static Mesh[] FromBabylonModel(string fileName) {
            var j = JsonValue.FromFile(fileName);
            var meshes = new List<Mesh>();

            foreach (var jmesh in j["meshes"].Values) {
                var mesh = new Mesh();

                var uvCount = jmesh["uvCount"].ValueOf();
                var verticesStep = new[] { 6, 8, 10 }[uvCount];
                var vertices = new List<Vector>();
                var verticesArray = jmesh["vertices"];
                var verticesCount = verticesArray.Count / verticesStep;
                for (var i = 0; i < verticesCount; i++) {
                    var x = (float)verticesArray[i * verticesStep].ValueOf();
                    var y = (float)verticesArray[i * verticesStep + 1].ValueOf();
                    var z = (float)verticesArray[i * verticesStep + 2].ValueOf();
                    vertices.Add(new Vector(x, y, z));
                }
                mesh.Vertices = vertices.ToArray();

                var indices = jmesh["indices"];
                var surfaceCount = indices.Count / 3;
                var surfaceList = new List<Surface>();
                for (var i = 0; i < surfaceCount; i++) {
                    var a = indices[i * 3].ValueOf();
                    var b = indices[i * 3 + 1].ValueOf();
                    var c = indices[i * 3 + 2].ValueOf();
                    surfaceList.Add(new Surface { A = a, B = b, C = c });
                }
                mesh.Surfaces = surfaceList.ToArray();

                var position = jmesh["position"];
                mesh.Position = new Vector(
                    (float)position[0].ValueOf(),
                    (float)position[1].ValueOf(),
                    (float)position[2].ValueOf()
                    );
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
    }
}