using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Triangle {

    public static void ResetIndex() { m_index = 0; }

    public Triangle() {
        Index = m_index;
        m_index++;
    }

    public Triangle(Triangle src) {
        A = src.A;
        B = src.B;
        C = src.C;
        AB = src.AB;
        BC = src.BC;
        CA = src.CA;
        Index = m_index;
        m_index++;
    }

    public Triangle(VertexOLD a, VertexOLD b, VertexOLD c) {
        A = a;
        B = b;
        C = c;
        Index = m_index;
        m_index++;
    }

    #region Triangle Data
    protected VertexOLD m_a = null;
    protected VertexOLD m_b = null;
    protected VertexOLD m_c = null;

    protected float m_abLen = 0;
    protected float m_bcLen = 0;
    protected float m_caLen = 0;
    protected bool m_abLenCalcd = false;
    protected bool m_bcLenCalcd = false;
    protected bool m_caLenCalcd = false;

    protected bool m_abDet = false;
    protected bool m_bcDet = false;
    protected bool m_caDet = false;
    protected bool m_abDetCalcd = false;
    protected bool m_bcDetCalcd = false;
    protected bool m_caDetCalcd = false;

    protected static int m_index = 0;

    protected Triangle m_ab = null;
    protected Triangle m_bc = null;
    protected Triangle m_ca = null;

    protected bool m_centreComputed = false;
    protected VertexOLD m_centre = null;
    #endregion

    public int Index { get; protected set; }

    public int RegionCode { get; set; }

    public VertexOLD Centre {
        get {
            if (m_centreComputed) {
                return m_centre;
            }
            m_centre = new VertexOLD(
                (A.x + B.x + C.x) / 3f,
                (A.y + B.y + C.y) / 3f,
                (A.z + B.z + C.z) / 3f);

            float delta = m_centre.DeltaSquaredXY(A);
            float tmp = m_centre.DeltaSquaredXY(B);

            delta = delta > tmp ? delta : tmp;
            tmp = m_centre.DeltaSquaredXY(C);
            delta = delta > tmp ? delta : tmp;

            FarthestFromCentre = delta;
            m_centreComputed = true;

            return m_centre;
        }
    }

    public float FarthestFromCentre { get; protected set; }

    #region Vertices
    public VertexOLD A {
        get { return m_a; }
        set {
            if (m_a == value) {
                return;
            }
            m_abDetCalcd = false;
            m_caDetCalcd = false;
            m_abLenCalcd = false;
            m_caLenCalcd = false;
            m_centreComputed = false;
            m_a = value;
        }
    }

    public VertexOLD B {
        get { return m_b; }
        set {
            if (m_b == value) {
                return;
            }
            m_abDetCalcd = false;
            m_bcDetCalcd = false;
            m_abLenCalcd = false;
            m_bcLenCalcd = false;
            m_centreComputed = false;
            m_b = value;
        }
    }

    public VertexOLD C {
        get { return m_c; }
        set {
            if (m_c == value) {
                return;
            }
            m_caDetCalcd = false;
            m_bcDetCalcd = false;
            m_caLenCalcd = false;
            m_bcLenCalcd = false;
            m_centreComputed = false;
            m_c = value;
        }
    }
    #endregion

    public Triangle AB { get { return m_ab; } set { m_ab = value; } }
    public Triangle BC { get { return m_bc; } set { m_bc = value; } }
    public Triangle CA { get { return m_ca; } set { m_ca = value; } }

    protected bool abDet {
        get {
            if (!m_abDetCalcd) {
                m_abDet = VertexTest(A, B, C);
            }
            return m_abDet;
        }
    }

    protected bool bcDet {
        get {
            if (!m_bcDetCalcd) {
                m_bcDet = VertexTest(B, C, A);
            }
            return m_bcDet;
        }
    }

    protected bool caDet {
        get {
            if (!m_caDetCalcd) {
                m_caDet = VertexTest(C, A, B);
            }
            return m_caDet;
        }
    }

    protected bool VertexTest(VertexOLD la, VertexOLD lb, VertexOLD t) {
        if (la.x == lb.x) {
            return t.x > la.x;
        }

        if (la.y == lb.y) {
            return t.y > la.y;
        }

        float m = (la.y - lb.y) / (la.x - lb.x);
        float b = la.y - (m * la.x);

        return (m * t.x + b - t.y) > 0;
    }

    public bool Contains(VertexOLD t) {
        float delta = t.DeltaSquaredXY(Centre);

        if (delta > FarthestFromCentre) {
            return false;
        }

        if (abDet != VertexTest(A, B, t)) {
            return false;
        }

        if (bcDet != VertexTest(B, C, t)) {
            return false;
        }

        if (caDet != VertexTest(C, A, t)) {
            return false;
        }

        return true;
    }

    #region Get Lengths
    public float AB_Length {
        get {
            if (m_abLenCalcd == true) {
                return m_abLen;
            }

            if ((A == null) || (B == null)) {
                return -1;
            }

            m_abLen = A.DeltaSquaredXY(B);
            m_abLenCalcd = true;

            return m_abLen;
        }
    }

    public float BC_Length {
        get {
            if (m_bcLenCalcd == true) {
                return m_bcLen;
            }

            if ((B == null) || (B == null)) {
                return -1;
            }

            m_bcLen = B.DeltaSquaredXY(C);
            m_bcLenCalcd = true;

            return m_bcLen;
        }
    }

    public float CA_Length {
        get {
            if (m_caLenCalcd == true) {
                return m_caLen;
            }

            if ((C == null) || (A == null)) {
                return -1;
            }

            m_caLen = C.DeltaSquaredXY(A);
            m_caLenCalcd = true;

            return m_caLen;
        }
    }
    #endregion

    public float Area {
        get {
            float a = AB_Length;
            float b = BC_Length;
            float c = CA_Length;

            a = (float)System.Math.Sqrt(a);
            b = (float)System.Math.Sqrt(b);
            c = (float)System.Math.Sqrt(c);

            float s = 0.5f * (a + b + c);

            return (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

    public float Edge_Length(int i) {

        if (i < 0) {

            i += 3;

        } else if (i > 2) {

            i -= 3;

        }

        if (i == 0) {
            return AB_Length;
        } else if (i == 1) {
            return BC_Length;
        } else {
            return CA_Length;
        }
    }

    public VertexOLD OppositeOfEdge(int i) {

        if (i < 0) {
            i += 3;
        } else if (i > 2) {
            i -= 3;
        }

        if (i == 0) {
            return C;
        } else if (i == 1) {
            return A;
        } else {
            return B;
        }
    }

    public void SetVertex(int i, VertexOLD v) {
        
        if (i < 0) {
            i += 3;
        } else if (i > 2) {
            i -= 3;
        }

        if (i == 0) {
            A = v;
        } else if (i == 1) {
            B = v;
        } else if (i == 2) {
            C = v;
        }
    }

    public float VertexCosineAngle(int i) {

        if (i < 0) {
            i += 3;
        } else if (i > 2) {
            i -= 3;
        }

        float dx1 = 0;
        float dx2 = 0;
        float dy1 = 0;
        float dy2 = 0;

        if (i == 0) {
            dx1 = B.x - A.x;
            dy1 = B.y - A.y;
            dx2 = C.x - A.x;
            dy2 = C.y - A.y;
        } else {
            if (i == 1) {
                dx1 = C.x - B.x;
                dy1 = C.y - B.y;
                dx2 = A.x - B.x;
                dy2 = A.y - B.y;
            } else {
                dx1 = A.x - C.x;
                dy1 = A.y - C.y;
                dx2 = B.x - C.x;
                dy2 = B.y - C.y;
            }
        }

        float mag1 = (dx1 * dx1) + (dy1 * dy1);
        float mag2 = (dx2 * dx2) + (dy2 * dy2);
        float mag = (float)System.Math.Sqrt(mag1 * mag2);
        float dot = (float)((dx1 * dx2) + (dy1 * dy2)) / mag;

        return dot;
    }

    public float VertexAngleRadians(int i) {
        return (float)System.Math.Acos(VertexCosineAngle(i));
    }

    public bool Inside(System.Drawing.RectangleF region) {
        if (!A.InsideXY(region)) {
            return false;
        }

        if (!B.InsideXY(region)) {
            return false;
        }

        if (!C.InsideXY(region)) {
            return false;
        }

        return true;
    }

    public void RepairEdges(Triangle a) {
        if (this.Index == a.Index) {
            return;
        }

    }

    protected bool BothIn(Triangle t, VertexOLD a, VertexOLD b) {
        if (a == A) {
            if (b == B) { AB = t; return true; }
            if (b == C) { CA = t; return true; }
        } else if (a == B) {
            if (b == A) { AB = t; return true; }
            if (b == C) { BC = t; return true; }
        } else if (a == C) {
            if (b == A) { CA = t; return true; }
            if (b == B) { BC = t; return true; }
        }

        return false;
    }

    public VertexOLD GetVertex(int i) {
        if (i < 0) {
            i += 3;
        } else if (i > 2) {
            i -= 3;
        }

        if (i == 0) { return A; }
        if (i == 1) { return B; }

        return C;
    }

    public void SetEdge(int i, Triangle t) {
        if (i < 0) {
            i += 3;
        } else if (i > 2) {
            i -= 3;
        }

        if (i == 0) {
            AB = t;
        } else if (i == 1) {
            BC = t;
        } else if (i == 2) {
            CA = t;
        }
    }

    public Triangle Edge(int i) {
        if (i == 0) {
            return AB;
        } else if (i == 1) {
            return BC;
        } else {
            return CA;
        }
    }
}

