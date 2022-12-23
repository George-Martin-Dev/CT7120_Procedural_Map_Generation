public static class ForcePos
{
    /// <summary>
    /// Function for making any negative values in a list of floats, positive.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float[] pos(params float[] value) {
        for (int i = 0; i < value.Length; i++) {
            if (value[i] < 0) {
                value[i] *= -1;
            }
        }
        return value;
    }
}
