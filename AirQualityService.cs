using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AirQualityMonitoring2
{
    /// <summary>
    /// Модель даних якості повітря
    /// </summary>
    public class AirQualityData
    {
        public string DistrictName { get; set; }
        public int AQI { get; set; }
        public double PM25 { get; set; }
        public double PM10 { get; set; }
        public double NO2 { get; set; }
        public double SO2 { get; set; }
        public double CO { get; set; }
        public double O3 { get; set; }
        public DateTime Timestamp { get; set; }
        public string WeatherCondition { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }

    /// <summary>
    /// Сервіс для отримання даних про якість повітря
    /// </summary>
    public class AirQualityService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private readonly Random random = new Random();

        // Координати центрів районів Києва
        private readonly Dictionary<string, (double lat, double lon)> districtCoordinates = new Dictionary<string, (double, double)>
        {
            ["Голосіївський"] = (50.3961, 30.5080),
            ["Дарницький"] = (50.4020, 30.6282),
            ["Деснянський"] = (50.5298, 30.5981),
            ["Дніпровський"] = (50.4851, 30.5982),
            ["Оболонський"] = (50.5010, 30.4811),
            ["Печерський"] = (50.4223, 30.5434),
            ["Подільський"] = (50.4734, 30.4562),
            ["Святошинський"] = (50.4582, 30.3659),
            ["Солом'янський"] = (50.4176, 30.4543),
            ["Шевченківський"] = (50.4549, 30.5033)
        };

        public AirQualityService(string apiKey = null)
        {
            this.httpClient = new HttpClient();
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Отримати дані якості повітря для всіх районів
        /// </summary>
        public async Task<List<AirQualityData>> GetAllDistrictsDataAsync()
        {
            var results = new List<AirQualityData>();

            foreach (var district in districtCoordinates)
            {
                var data = await GetDistrictDataAsync(district.Key);
                results.Add(data);
            }

            return results;
        }

        /// <summary>
        /// Отримати дані якості повітря для конкретного району
        /// </summary>
        public async Task<AirQualityData> GetDistrictDataAsync(string districtName)
        {
            if (!districtCoordinates.ContainsKey(districtName))
            {
                throw new ArgumentException($"Невідомий район: {districtName}");
            }

            var (lat, lon) = districtCoordinates[districtName];

            // Якщо є API ключ, використовуємо реальний API
            if (!string.IsNullOrEmpty(apiKey))
            {
                return await GetRealDataAsync(districtName, lat, lon);
            }
            else
            {
                // Інакше генеруємо симульовані дані
                return GenerateSimulatedData(districtName);
            }
        }

        /// <summary>
        /// Отримати реальні дані з API (приклад для OpenWeatherMap Air Pollution API)
        /// </summary>
        private async Task<AirQualityData> GetRealDataAsync(string districtName, double lat, double lon)
        {
            try
            {
                // Приклад URL для OpenWeatherMap Air Pollution API
                string url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={lat}&lon={lon}&appid={apiKey}";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    return new AirQualityData
                    {
                        DistrictName = districtName,
                        AQI = data.list[0].main.aqi,
                        PM25 = data.list[0].components.pm2_5,
                        PM10 = data.list[0].components.pm10,
                        NO2 = data.list[0].components.no2,
                        SO2 = data.list[0].components.so2,
                        CO = data.list[0].components.co,
                        O3 = data.list[0].components.o3,
                        Timestamp = DateTime.Now,
                        WeatherCondition = "Ясно",
                        Temperature = 20,
                        Humidity = 60,
                        WindSpeed = 3.5
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні даних з API: {ex.Message}");
            }

            // У випадку помилки повертаємо симульовані дані
            return GenerateSimulatedData(districtName);
        }

        /// <summary>
        /// Генерувати симульовані дані для тестування
        /// </summary>
        private AirQualityData GenerateSimulatedData(string districtName)
        {
            // Базові значення з варіаціями для різних районів
            var baseValues = new Dictionary<string, int>
            {
                ["Голосіївський"] = 45,  
                ["Дарницький"] = 85,     
                ["Деснянський"] = 70,    
                ["Дніпровський"] = 75,   
                ["Оболонський"] = 60,    
                ["Печерський"] = 65,     
                ["Подільський"] = 80,    
                ["Святошинський"] = 90,  
                ["Солом'янський"] = 95,  
                ["Шевченківський"] = 70  
            };

            int baseAQI = baseValues.ContainsKey(districtName) ? baseValues[districtName] : 70;

            // Додаємо випадкові варіації
            int variation = random.Next(-15, 20);
            int aqi = Math.Max(0, Math.Min(500, baseAQI + variation));

            // Розрахунок компонентів на основі AQI
            return new AirQualityData
            {
                DistrictName = districtName,
                AQI = aqi,
                PM25 = Math.Round(aqi * 0.5 + random.NextDouble() * 10, 2),
                PM10 = Math.Round(aqi * 0.8 + random.NextDouble() * 15, 2),
                NO2 = Math.Round(aqi * 0.3 + random.NextDouble() * 5, 2),
                SO2 = Math.Round(aqi * 0.2 + random.NextDouble() * 3, 2),
                CO = Math.Round(aqi * 15 + random.NextDouble() * 100, 2),
                O3 = Math.Round(aqi * 0.4 + random.NextDouble() * 8, 2),
                Timestamp = DateTime.Now,
                WeatherCondition = GetRandomWeatherCondition(),
                Temperature = Math.Round(15 + random.NextDouble() * 15, 1),
                Humidity = random.Next(40, 80),
                WindSpeed = Math.Round(1 + random.NextDouble() * 8, 1)
            };
        }

        private string GetRandomWeatherCondition()
        {
            string[] conditions = { "Ясно", "Хмарно", "Частково хмарно", "Туман", "Дощ", "Злива" };
            return conditions[random.Next(conditions.Length)];
        }

        /// <summary>
        /// Розрахувати AQI на основі концентрацій забруднювачів
        /// </summary>
        public static int CalculateAQI(double pm25, double pm10, double no2, double so2, double co, double o3)
        {
            // Спрощений розрахунок AQI (в реальності використовується складніша формула EPA)
            var aqiValues = new List<int>
            {
                CalculatePM25AQI(pm25),
                CalculatePM10AQI(pm10),
                CalculateNO2AQI(no2),
                CalculateSO2AQI(so2),
                CalculateCOAQI(co),
                CalculateO3AQI(o3)
            };

            return aqiValues.Max();
        }

        private static int CalculatePM25AQI(double concentration)
        {
            if (concentration <= 12.0) return LinearScale(concentration, 0, 12.0, 0, 50);
            if (concentration <= 35.4) return LinearScale(concentration, 12.1, 35.4, 51, 100);
            if (concentration <= 55.4) return LinearScale(concentration, 35.5, 55.4, 101, 150);
            if (concentration <= 150.4) return LinearScale(concentration, 55.5, 150.4, 151, 200);
            if (concentration <= 250.4) return LinearScale(concentration, 150.5, 250.4, 201, 300);
            return LinearScale(concentration, 250.5, 500.4, 301, 500);
        }

        private static int CalculatePM10AQI(double concentration)
        {
            if (concentration <= 54) return LinearScale(concentration, 0, 54, 0, 50);
            if (concentration <= 154) return LinearScale(concentration, 55, 154, 51, 100);
            if (concentration <= 254) return LinearScale(concentration, 155, 254, 101, 150);
            if (concentration <= 354) return LinearScale(concentration, 255, 354, 151, 200);
            if (concentration <= 424) return LinearScale(concentration, 355, 424, 201, 300);
            return LinearScale(concentration, 425, 604, 301, 500);
        }

        private static int CalculateNO2AQI(double concentration)
        {
            if (concentration <= 40) return LinearScale(concentration, 0, 40, 0, 50);
            if (concentration <= 80) return LinearScale(concentration, 41, 80, 51, 100);
            if (concentration <= 180) return LinearScale(concentration, 81, 180, 101, 150);
            if (concentration <= 280) return LinearScale(concentration, 181, 280, 151, 200);
            if (concentration <= 400) return LinearScale(concentration, 281, 400, 201, 300);
            return LinearScale(concentration, 401, 800, 301, 500);
        }

        private static int CalculateSO2AQI(double concentration)
        {
            if (concentration <= 20) return LinearScale(concentration, 0, 20, 0, 50);
            if (concentration <= 80) return LinearScale(concentration, 21, 80, 51, 100);
            if (concentration <= 250) return LinearScale(concentration, 81, 250, 101, 150);
            if (concentration <= 350) return LinearScale(concentration, 251, 350, 151, 200);
            return LinearScale(concentration, 351, 500, 201, 300);
        }

        private static int CalculateCOAQI(double concentration)
        {
            if (concentration <= 4400) return LinearScale(concentration, 0, 4400, 0, 50);
            if (concentration <= 9400) return LinearScale(concentration, 4500, 9400, 51, 100);
            if (concentration <= 12400) return LinearScale(concentration, 9500, 12400, 101, 150);
            if (concentration <= 15400) return LinearScale(concentration, 12500, 15400, 151, 200);
            if (concentration <= 30400) return LinearScale(concentration, 15500, 30400, 201, 300);
            return LinearScale(concentration, 30500, 50400, 301, 500);
        }

        private static int CalculateO3AQI(double concentration)
        {
            if (concentration <= 60) return LinearScale(concentration, 0, 60, 0, 50);
            if (concentration <= 100) return LinearScale(concentration, 61, 100, 51, 100);
            if (concentration <= 140) return LinearScale(concentration, 101, 140, 101, 150);
            if (concentration <= 180) return LinearScale(concentration, 141, 180, 151, 200);
            if (concentration <= 240) return LinearScale(concentration, 181, 240, 201, 300);
            return LinearScale(concentration, 241, 380, 301, 500);
        }

        private static int LinearScale(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (int)Math.Round(((value - inMin) / (inMax - inMin)) * (outMax - outMin) + outMin);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}