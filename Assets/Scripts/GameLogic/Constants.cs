using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVals
{
    public static string[] cities = {"Johannesburg", "Khartoum", "Kinshasa", "Lagos", "Sao Paulo","Buenos Aires","Bogata","Lima","Santiago","Mexico City","Miami",
        "Los Angeles","San Francisco","Chicago","Atlanta","Washington"};
    public const int JOHANNESBURG = 0;
    public const int KHARTOUM = 1;
    public const int KINSHASA = 2;
    public const int LAGOS = 3;
    public const int SAO_PAULO = 4;
    public const int BUENOS_AIRES = 5;
    public const int BOGATA = 6;
    public const int LIMA = 7;
    public const int SANTIAGO = 8;
    public const int MEXICO_CITY = 9;
    public const int MIAMI = 10;
    public const int LOS_ANGELES = 11;
    public const int SAN_FRANCISCO = 12;
    public const int CHICAGO = 13;
    public const int ATLANTA = 14;
    public const int WASHINGTON = 15;

    public const int EPIDEMIC = 100;

    
    public static int[][] locNeighbours = 
        new int[][] {new int[]{KHARTOUM,KINSHASA},new int[]{},new int[]{JOHANNESBURG,KHARTOUM,LAGOS},
                            new int[]{JOHANNESBURG,KINSHASA,SAO_PAULO},new int[]{},new int[]{SAO_PAULO,BOGATA},
                            new int[]{SAO_PAULO,BUENOS_AIRES,LIMA,MEXICO_CITY,MIAMI},new int[]{BOGATA,SANTIAGO,MEXICO_CITY},new int[]{LIMA},
                            new int[]{BOGATA,LIMA,MIAMI,LOS_ANGELES,CHICAGO},new int[]{BOGATA,MEXICO_CITY,ATLANTA,WASHINGTON},new int[]{},
                            new int[]{},new int[]{},new int[]{MIAMI,CHICAGO,WASHINGTON},
                            new int[]{}
                        };

    public const int BASE_INFECTION_RATE = 2;
    public const int OUTBREAK_THRESHOLD = 3;
    public const int CUBES_PER_EPIDEMIC_INFECT = 3;
    public const int INITIAL_INFECTION_ROUNDS = 3;
    public const int CARDS_PER_INITIAL_INFECTION_ROUND = 3;

    public enum Colour {YELLOW, BLUE, BLACK, RED};
    
}

