using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mesh {
    protected int m_recursion = 4;
    protected List<Vertex> m_points = new List<Vertex>();
    protected List<Triangle> m_facets = new List<Triangle>();
    protected System.Drawing.RectangleF m_bounds = new System.Drawing.RectangleF(0, 0, 640, 480);

    #region getter setters
    public List<Vertex> points {
        get { return m_points; }
        set { m_points = value; }
    }

    public List<Triangle> facets {
        get { return m_facets; }
        set { m_facets = value; }
    }

    public System.Drawing.RectangleF bounds {
        get { return m_bounds; }
        set { m_bounds = value; }
    }

    public int recursion {
        get { return m_recursion; }
        set { if (value < 0) value = 0; m_recursion = value; }
    }
    #endregion

    public int[] GetVertexIndicies() {
        int[] indicies = new int[3 * facets.Count];
        int k = 0;
        for (int i = 0; i < m_points.Count; i++) {
            m_points[i].index = i;
        }

        for (int i = 0; i < facets.Count; i++) {
            indicies[k++] = facets[i].A.index;
            indicies[k++] = facets[i].B.index;
            indicies[k++] = facets[i].C.index;
        }
        return indicies;
    }

    public void Compute(List<Vertex> set, System.Drawing.RectangleF bounds) {
        Setup(bounds);

        for (int i = 0; i < set.Count; i++) {
            
        }
    }

    public void Append(Vertex v) {
        for (int i = 0; i < facets.Count; i++) {
            if (facets[i].Contains(v)) {
                Insert(v, facets[i]);
            }
        }
    }

    public void Setup(System.Drawing.RectangleF a_bounds) {
        Triangle.ResetIndex();
        facets.Clear();
        points.Clear();
        bounds = a_bounds;

        Vertex tl = new Vertex(bounds.Left, bounds.Top, 0);
        Vertex tr = new Vertex(bounds.Right, bounds.Top, 0);
        Vertex bl = new Vertex(bounds.Left, bounds.Bottom, 0);
        Vertex br = new Vertex(bounds.Right, bounds.Bottom, 0);

        Triangle t1 = new Triangle();
        Triangle t2 = new Triangle();

        t1.A = bl;
        t1.B = tr;
        t1.C = tl;
        t2.A = bl;
        t2.B = br;
        t2.C = tr;
        t1.AB = t2;
        t2.CA = t1;
        facets.Add(t1);
        facets.Add(t2);
    }

    protected void Insert(Vertex v, Triangle old) {
        if ((old.A.x == v.x) && (old.A.y == v.y)) {
            return;
        }

        if ((old.B.x == v.x) && (old.B.y == v.y)) {
            return;
        }

        if ((old.C.x == v.x) && (old.C.y == v.y)) {
            return;
        }

        m_points.Add(v);

        Triangle ab = new Triangle(old);
        Triangle bc = new Triangle(old);
        Triangle ca = new Triangle(old);

        ab.C = v;
        bc.A = v;
        ca.B = v;

        ab.BC = bc;
        ab.CA = ca;
        bc.AB = ab;
        bc.CA = ca;
        ca.AB = ab;
        ca.BC = bc;

        Triangle[] ta = { ab.AB, bc.BC, ca.CA };
        Triangle[] tb = { ab, bc, ca };

        for (int j = 0; j < 3; j++) {
            if (ta[j] == null) {
                continue;
            }

            if (ta[j].Edge(0) == old) {
                ta[j].SetEdge(0, tb[j]);
                continue;
            }

            if (ta[j].Edge(1) == old) {
                ta[j].SetEdge(1, tb[j]);
                continue;
            }
            ta[j].SetEdge(2, tb[j]);
        }

        facets.Add(ab);
        facets.Add(bc);
        facets.Add(ca);
        facets.Remove(old);
    }

    void flipIfNeeded(Triangle a, Triangle b, int depth) {
        if (depth <= 0) {
            return;
        }

        if (a == null) {
            return;
        }

        if (b == null) {
            return;
        }

        depth--;

        int ai = 0;
        int bi = 0;

        if (a.Edge(1) == b) {
            ai = 1;
        }

        if (a.Edge(2) == b) {
            ai = 2;
        }

        if (b.Edge(1) == a) {
            bi = 1;
        }

        if (b.Edge(2) == a) {
            bi = 2;
        }

        int[] table = { 2, 0, 1 };
        int vai = table[ai];
        int vbi = table[bi];

        float fa = a.VertexAngleRadians(vai);
        float fb = b.VertexAngleRadians(vbi);

        if (fa + fb <= System.Math.PI) {
            return;
        }

        Triangle[] ts = { a.Edge(0), a.Edge(1), a.Edge(2), b.Edge(0), b.Edge(1), b.Edge(2) };

        Vertex aOp = a.OppositeOfEdge(ai);
        Vertex bOp = b.OppositeOfEdge(bi);

        a.SetVertex(ai + 1, bOp);
        b.SetVertex(bi + 1, aOp);

        a.AB = null;
        a.BC = null;
        a.CA = null;
        b.AB = null;
        b.BC = null;
        b.CA = null;

        for (int i = 0; i < 6; i++) {
            if (ts[i] == null) {
                continue;
            }

            ts[i].RepairEdges(a);
            ts[i].RepairEdges(b);
        }

        flipIfNeeded(a, a.Edge(ai + 1), depth);
        flipIfNeeded(b, b.Edge(bi + 1), depth);
        flipIfNeeded(a, a.Edge(ai + 2), depth);
        flipIfNeeded(b, b.Edge(bi + 2), depth);
    }
}

