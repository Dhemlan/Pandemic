﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVals
{
    public static string[] cities = {"Johannesburg", "Khartoum", "Kinshasa", "Lagos", "Sao Paulo","Buenos Aires","Bogata","Lima","Santiago","Mexico City","Miami",
        "Los Angeles","San Francisco","Chicago","Atlanta","Washington"};

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
    public const int OUTBREAK_COUNT_LIMIT = 8;
    public const int INITIAL_DISEASE_CUBE_COUNT = 20;

    public const float GENERIC_WAIT_TIME = 1.0f;

    public const string GAME_OVER_OUTBREAKS = "Frequent outbreaks have made the disease uncontainable\nGame Over";
    public const string GAME_OVER_CUBES = "One of the diseases has taken over\nGame Over";
    public const string GAME_OVER_CARDS = "You've ran out of time\n Game Over";

    public enum Colour {YELLOW, BLUE, BLACK, RED};
    
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
    public const int MONTREAL = 16;
    public const int NEW_YORK = 17;
    public const int MADRID = 18;
    public const int LONDON = 19;
    public const int PARIS = 20;
    public const int ESSEN = 21;
    public const int MILAN = 22;
    public const int ST_PETERSBURG = 23;
    public const int ALGIERS = 24;
    public const int CAIRO = 25;
    public const int ISTANBUL = 26;
    public const int BAGHDAD = 27;
    public const int MOSCOW = 28;
    public const int RIYADH = 29;
    public const int KARACHI = 30;
    public const int TEHRAN = 31;
    public const int DELHI = 32;
    public const int MUMBAI = 33;
    public const int CHENNAI = 34;
    public const int KOLKATA = 35;
    public const int BANGKOK = 36;
    public const int JAKARTA = 37;
    public const int HO_CHI_MINH_CITY = 38;
    public const int HONG_KONG = 39;
    public const int SYDNEY = 40;
    public const int MANILA = 41;
    public const int TAIPEI = 42;
    public const int OSAKA = 43;
    public const int TOKYO = 44;
    public const int SHANGHAI = 45;
    public const int BEIJING = 46;
    public const int SEOUL = 47;

    public const int EPIDEMIC = 100;
    public const int ONE_QUIET_NIGHT = 101;
    public const int AIRLIFT = 102;
    public const int RESILIENT_POPULATION = 103;
    public const int FORECAST = 104;
    public const int GOVERNMENT_GRANT = 105;
}

