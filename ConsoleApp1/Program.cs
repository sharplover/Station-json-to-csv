using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace JsonToCsv
{
    public class MapStation
    {
        public double x { get; set; }
        public double y { get; set; }
        public StationInfo Station { get; set; }
        public string StationName { get; set; } // Исправление: добавляем свойство StationName
        public int SignaturePosition { get; set; }
        public bool IsLocoDepot { get; set; }
        public string LocoDepotName { get; set; }
        public int DepotType { get; set; }
        public bool IsMaintenancePoint { get; set; }
        public object MaintenancePointName { get; set; }
        public bool IsProvideMaintenance { get; set; }
        public bool IsProvideRoutineRepair { get; set; }
        public bool HasProductionArea { get; set; }
        public int ProductionAreaType { get; set; }
        public int CompaniesBadgesPosition { get; set; }
    }

    public class StationInfo
    {
        public int IdStation { get; set; }
    }

    public class Station
    {
        public int stanId { get; set; }
        public int stanTipId { get; set; }
        public string name { get; set; }
        public int esr { get; set; }
        public int? stClassId { get; set; } // Используем Nullable int
        public int? harId { get; set; } // Используем Nullable int
        public bool isImportant { get; set; }
        public bool isJunc { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class StationList
    {
        public int total { get; set; }
        public List<Station> stations { get; set; }
    }

    public class CombinedStation
    {
        public int stanId { get; set; }
        public int stanTipId { get; set; }
        public string name { get; set; }
        public int esr { get; set; }
        public int stClassId { get; set; }
        public int harId { get; set; }
        public bool isImportant { get; set; }
        public bool isJunc { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public int SignaturePosition { get; set; }
        public bool IsLocoDepot { get; set; }
        public string LocoDepotName { get; set; }
        public int DepotType { get; set; }
        public bool IsMaintenancePoint { get; set; }
        public object MaintenancePointName { get; set; }
        public bool IsProvideMaintenance { get; set; }
        public bool IsProvideRoutineRepair { get; set; }
        public bool HasProductionArea { get; set; }
        public int ProductionAreaType { get; set; }
        public int CompaniesBadgesPosition { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var mapJson = File.ReadAllText("map.json");
            var stationsJson = File.ReadAllText("stations.json");

            var mapData = JsonConvert.DeserializeObject<Dictionary<string, List<MapStation>>>(mapJson)["Stations"];
            var stationsList = JsonConvert.DeserializeObject<StationList>(stationsJson);

            var combinedStations = from map in mapData
                                   join station in stationsList.stations
                                   on map.Station.IdStation equals station.esr
                                   select new CombinedStation
                                   {
                                       stanId = station.stanId,
                                       stanTipId = station.stanTipId,
                                       name = map.StationName,
                                       esr = map.Station.IdStation,
                                       stClassId = station.stClassId.GetValueOrDefault(), // Используем GetValueOrDefault
                                       harId = station.harId.GetValueOrDefault(), // Используем GetValueOrDefault
                                       isImportant = station.isImportant,
                                       isJunc = station.isJunc,
                                       x = map.x / 1000.0,
                                       y = map.y / 1000.0,
                                       SignaturePosition = map.SignaturePosition,
                                       IsLocoDepot = map.IsLocoDepot,
                                       LocoDepotName = map.LocoDepotName,
                                       DepotType = map.DepotType,
                                       IsMaintenancePoint = map.IsMaintenancePoint,
                                       MaintenancePointName = map.MaintenancePointName,
                                       IsProvideMaintenance = map.IsProvideMaintenance,
                                       IsProvideRoutineRepair = map.IsProvideRoutineRepair,
                                       HasProductionArea = map.HasProductionArea,
                                       ProductionAreaType = map.ProductionAreaType,
                                       CompaniesBadgesPosition = map.CompaniesBadgesPosition
                                   };

            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {

                foreach (var station in combinedStations)
                {
                    Console.WriteLine($"LocoDepotName: {station.LocoDepotName}");
                }

                csv.WriteRecords(combinedStations);
            }
        }
    }
}
