using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Vertex {

    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public int index { get; set; } 

    public Vertex() {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vertex(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float DeltaSquaredXY(Vertex t) {
        float dx = (x - t.x);
        float dy = (y - t.y);
        return (dx * dx) + (dy * dy);
    }

    public float DeltaSquared(Vertex t) {
        float dx = (x - t.x);
        float dy = (y - t.y);
        float dz = (z - t.z);
        return (dx * dx) + (dy * dy) + (dz * dz);
    }

    public float DistanceXY(Vertex t) {
        return (float)System.Math.Sqrt(DeltaSquaredXY(t));
    }

    public float Distance(Vertex t) {
        return (float)System.Math.Sqrt(DeltaSquared(t));
    }

    public bool InsideXY(System.Drawing.RectangleF region) {
        if (x < region.Left) {
            return false;
        }

        if (x > region.Right) {
            return false;
        }

        if (y < region.Top) {
            return false;
        }

        if (y > region.Bottom) {
            return false;
        }

        return true;
    }
}

