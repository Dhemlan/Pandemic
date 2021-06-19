using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vals
{
    public static string[] cities = {"Johannesburg","Khartoum","Kinshasa","Lagos","Sao Paulo","Buenos Aires","Bogata","Lima","Santiago","Mexico City","Miami",
        "Los Angeles","San Francisco","Chicago","Atlanta","Washington","Montreal","New York","Madrid","London","Paris","Essen","Milan","St Petersburg","Algiers",
        "Cairo","Istanbul","Baghdad","Moscow","Riyadh","Karachi","Tehran","Delhi","Mumbai","Chennai","Kolkata","Bangkok","Jakarta","Ho ChiMinh City","Hong Kong",
        "Sydney","Manila","Taipei","Osaka","Tokyo","Shanghai","Beijing","Seoul"};

    public static int[][] locNeighbours = 
        new int[][] {new int[]{KHARTOUM,KINSHASA},new int[]{JOHANNESBURG,KINSHASA,LAGOS,CAIRO},new int[]{JOHANNESBURG,KHARTOUM,LAGOS},
                            new int[]{JOHANNESBURG,KINSHASA,SAO_PAULO},new int[]{LAGOS,BUENOS_AIRES,BOGATA,MADRID},new int[]{SAO_PAULO,BOGATA},
                            new int[]{SAO_PAULO,BUENOS_AIRES,LIMA,MEXICO_CITY,MIAMI},new int[]{BOGATA,SANTIAGO,MEXICO_CITY},new int[]{LIMA},
                            new int[]{BOGATA,LIMA,MIAMI,LOS_ANGELES,CHICAGO},new int[]{BOGATA,MEXICO_CITY,ATLANTA,WASHINGTON},new int[]{MEXICO_CITY,SAN_FRANCISCO,CHICAGO,SYDNEY},
                            new int[]{LOS_ANGELES,CHICAGO,MANILA,TOKYO},new int[]{MEXICO_CITY,LOS_ANGELES,SAN_FRANCISCO,ATLANTA,MONTREAL},new int[]{MIAMI,CHICAGO,WASHINGTON},
                            new int[]{MIAMI,ATLANTA,MONTREAL,NEW_YORK},new int[]{CHICAGO,WASHINGTON,NEW_YORK},new int[]{WASHINGTON,MONTREAL,LONDON,PARIS},
                            new int[]{SAO_PAULO,NEW_YORK,LONDON,PARIS,ALGIERS},new int[]{NEW_YORK,MADRID,PARIS,ESSEN},new int[]{MADRID,LONDON,ESSEN,MILAN,ALGIERS},
                            new int[]{LONDON,PARIS,MILAN,ST_PETERSBURG},new int[]{PARIS,ESSEN,ISTANBUL},new int[]{ESSEN,ISTANBUL,MOSCOW},
                            new int[]{MADRID,PARIS,CAIRO,ISTANBUL},new int[]{KHARTOUM,ALGIERS,ISTANBUL,BAGHDAD,RIYADH},new int[]{MILAN,ST_PETERSBURG,ALGIERS,CAIRO,BAGHDAD,MOSCOW},
                            new int[]{CAIRO,ISTANBUL,RIYADH,KARACHI,TEHRAN},new int[]{ST_PETERSBURG,ISTANBUL,TEHRAN},new int[]{CAIRO,BAGHDAD,KARACHI},
                            new int[]{BAGHDAD,RIYADH,TEHRAN,DELHI,MUMBAI},new int[]{BAGHDAD,MOSCOW,KARACHI,DELHI},new int[]{KARACHI,TEHRAN,MUMBAI,CHENNAI,KOLKATA},
                            new int[]{KARACHI,DELHI,CHENNAI},new int[]{DELHI,MUMBAI,KOLKATA,BANGKOK,JAKARTA},new int[]{DELHI,CHENNAI,BANGKOK,HONG_KONG},
                            new int[]{CHENNAI,KOLKATA,JAKARTA,HO_CHI_MINH_CITY, HONG_KONG},new int[]{CHENNAI,BANGKOK,HO_CHI_MINH_CITY,SYDNEY},new int[]{BANGKOK,JAKARTA,HONG_KONG,MANILA},
                            new int[]{KOLKATA,BANGKOK,HO_CHI_MINH_CITY,MANILA,TAIPEI,SHANGHAI},new int[]{LOS_ANGELES,JAKARTA,MANILA},new int[]{SAN_FRANCISCO,HO_CHI_MINH_CITY,HONG_KONG,SYDNEY,TAIPEI},
                            new int[]{HONG_KONG,MANILA,OSAKA,SHANGHAI},new int[]{TAIPEI,TOKYO},new int[]{SAN_FRANCISCO,OSAKA,SHANGHAI,SEOUL},
                            new int[]{HONG_KONG,TAIPEI,TOKYO,BEIJING,SEOUL},new int[]{SHANGHAI,SEOUL},new int[]{TOKYO,SHANGHAI,BEIJING}
                        };

    public static string[] actionIconLabels = new string[] {"RETRIEVE","MOVE\nOTHERS"};

    // Set-up
    public const int BASE_INFECTION_RATE = 2;
    public const int OUTBREAK_COUNT_LIMIT = 8;
    public const int INITIAL_INFECTION_ROUNDS = 3;
    public const int CARDS_PER_INITIAL_INFECTION_ROUND = 3; 
    public const int INITIAL_DISEASE_CUBE_COUNT = 24;
    public const int DISEASE_COUNT = 4;
    public static int[] INFECTION_RATE_TRACK = {2,2,2,3,3,4,4};
    
    public const int DEFAULT_HAND_LIMIT = 7;
    public const int DEFAULT_CARDS_TO_CURE = 5;
    public const int DEFAULT_RESEARCH_STATION_SUPPLY = 6;

    // Misc numbers
    public const int OUTBREAK_THRESHOLD = 3;
    public const int CUBES_PER_EPIDEMIC_INFECT = 3;

    public const int DISEASE_UNCURED = 0;
    public const int DISEASE_CURED = 1;
    public const int DISEASE_ERADICATED = 2;
    
    // Code-related values
    public static bool proceed = false;
    public static bool continueGameFlow = true;
    public static bool continueAfterSelect = false;
    public static bool continueAfterMultiSelect = false;
    public static bool oneQuietNightActive = false;
    public static bool removeResearchStation = false;
    public static bool eventResolving = false;
    public static int cardResolving = -1;
    public const int MAX_HAND_SIZE = 10;
    public const float GENERIC_WAIT_TIME = 0.75f;
    

    public const int SELECTABLE_PLAYER_CARD = 0;
    public const int PLAYER_CARD = 1;
    public const int SELECTABLE_INFECTION_CARD = 2;
    public const int INFECTION_CARD = 3;
    public const int SELECTABLE_DISEASE = 4;
    public const int SELECTABLE_PLAYER = 5;
    public const int EVENT_CARD = 6;

    public const string GAME_OVER_OUTBREAKS = "Frequent outbreaks have made the disease uncontainable\nGame Over";
    public const string GAME_OVER_CUBES = "One of the diseases has taken over\nGame Over";
    public const string GAME_OVER_CARDS = "You've ran out of time\n Game Over";
    public const string GAME_OVER_CURES = "Cures have been found for all disease\nThe world is saved!";

    public enum GameOver {CUBES, OUTBREAKS, CARDS, WIN};
    public enum Colour {YELLOW, BLUE, BLACK, RED, EVENT, EPIDEMIC};

    public enum Phase {SET_UP, ACTION, DRAW, INFECTION};

    public static string[] ROLES = {"Contingency Planner", "Dispatcher","Medic","Operations Expert","Quarantine Specialist","Researcher","Scientist"};
    public const int CONTINGENCY_PLANNER = 0;
    public const int DISPATCHER = 1;
    public const int MEDIC = 2;
    public const int OPERATIONS_EXPERT = 3;
    public const int QUARANTINE_SPECIALIST = 4;
    public const int RESEARCHER = 5;
    public const int SCIENTIST = 6;

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

