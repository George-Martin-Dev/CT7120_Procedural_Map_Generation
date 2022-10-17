using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RandomNumber {
    private const int m = 34654567;
    private const int a = 1432664;
    private const int c = 23967989;
    private long last = 210803;

    public RandomNumber() {
        last = DateTime.Now.Ticks % m;
    }

    public RandomNumber(long seed) {
        last = seed;
    }

    public long Next() {
        last = ((a * last) + c) % m;

        return last;
    }

    public long Next(long maxValue) {
        return Next() % maxValue;
    }
}
