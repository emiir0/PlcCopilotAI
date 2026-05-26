using Microsoft.AspNetCore.Mvc;
using PlcCopilotAI.Models;
using System.Text;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PlcCopilotAI.Controllers
{
    public class PlcController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new PlcRequest());
        }

        [HttpPost]
        public IActionResult Generate(PlcRequest request)
        {
            string scenario = request.Scenario?.ToLower() ?? "";

            var plc = AnalyzeScenario(scenario);

            request.Result = GenerateFullReport(plc);

            return View("Index", request);
        }

        private PlcAnalysis AnalyzeScenario(string scenario)
        {
            return new PlcAnalysis
            {
                Scenario = scenario,

                MotorCount = GetNumberBeforeWord(scenario, "motor", 1),
                PumpCount = GetNumberBeforeWord(scenario, "pompa", 0),
                FanCount = GetNumberBeforeWord(scenario, "fan", 0),
                ValveCount = GetNumberBeforeWord(scenario, "valf", 0),
                SensorCount = GetNumberBeforeWord(scenario, "sensör", 0),

                TimerSecond = GetTimerSecond(scenario),
                CounterLimit = GetCounterLimit(scenario),

                HasConveyor = ContainsAny(scenario, "konveyör", "konveyor", "bant"),
                HasSensor = ContainsAny(scenario, "sensör", "sensor", "algılama", "ürün görünce", "ürün algılayınca"),
                HasEmergencyStop = ContainsAny(scenario, "acil stop", "emergency", "e-stop", "acil durdurma"),
                HasAlarm = ContainsAny(scenario, "alarm", "ikaz", "uyarı", "buzzer"),
                HasCounter = ContainsAny(scenario, "sayaç", "counter", "adet", "say"),
                HasStartStop = ContainsAny(scenario, "start", "stop", "başlat", "durdur"),
                HasTimer = ContainsAny(scenario, "timer", "zaman", "saniye", "gecikme", "sonra"),
                HasHmi = ContainsAny(scenario, "hmi", "hmı", "ekran", "panel"),
                HasAutoMode = ContainsAny(scenario, "otomatik", "auto"),
                HasManualMode = ContainsAny(scenario, "manuel", "manual"),
                HasLevel = ContainsAny(scenario, "seviye", "depo", "tank"),
                HasTemperature = ContainsAny(scenario, "sıcaklık", "ısı", "temperature"),
                HasPressure = ContainsAny(scenario, "basınç", "pressure"),
                HasTermic = ContainsAny(scenario, "termik", "overload", "aşırı akım"),
                HasReset = ContainsAny(scenario, "reset", "sıfırla")
            };
        }

        private string GenerateFullReport(PlcAnalysis plc)
        {
            var result = new StringBuilder();

            string projectType = GetProjectType(plc);

            result.AppendLine("PLC COPILOT AI - GELİŞMİŞ OTOMATİK PLC ANALİZ RAPORU");
            result.AppendLine("====================================================");
            result.AppendLine();
            result.AppendLine($"PROJE TİPİ : {projectType}");
            result.AppendLine();

            AppendScenario(result, plc);
            AppendDetectedParameters(result, plc);
            AppendIoList(result, plc);
            AppendOperationLogic(result, plc);
            AppendStructuredTextCode(result, plc);
            AppendLadderExplanation(result, plc);
            AppendSafetyAnalysis(result, plc);
            AppendHmiSuggestions(result, plc);
            AppendTestScenario(result, plc);
            AppendDevelopmentSuggestions(result, plc);

            return result.ToString();
        }

        private void AppendScenario(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("1. ALGILANAN SENARYO");
            result.AppendLine("--------------------");
            result.AppendLine(string.IsNullOrWhiteSpace(plc.Scenario) ? "Senaryo girilmedi." : plc.Scenario);
            result.AppendLine();
        }

        private void AppendDetectedParameters(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("2. ALGILANAN PARAMETRELER");
            result.AppendLine("-------------------------");
            result.AppendLine($"Motor Sayısı        : {plc.MotorCount}");
            result.AppendLine($"Pompa Sayısı        : {plc.PumpCount}");
            result.AppendLine($"Fan Sayısı          : {plc.FanCount}");
            result.AppendLine($"Valf Sayısı         : {plc.ValveCount}");
            result.AppendLine($"Sensör Sayısı       : {(plc.SensorCount > 0 ? plc.SensorCount : (plc.HasSensor ? 1 : 0))}");
            result.AppendLine($"Timer Kullanımı     : {(plc.HasTimer ? "Var" : "Yok")}");
            result.AppendLine($"Timer Süresi        : {plc.TimerSecond} saniye");
            result.AppendLine($"Sayaç Kullanımı     : {(plc.HasCounter ? "Var" : "Yok")}");
            result.AppendLine($"Sayaç Limiti        : {plc.CounterLimit}");
            result.AppendLine($"Acil Stop           : {(plc.HasEmergencyStop ? "Var" : "Yok")}");
            result.AppendLine($"Alarm               : {(plc.HasAlarm ? "Var" : "Yok")}");
            result.AppendLine($"Konveyör            : {(plc.HasConveyor ? "Var" : "Yok")}");
            result.AppendLine($"Otomatik Mod        : {(plc.HasAutoMode ? "Var" : "Yok")}");
            result.AppendLine($"Manuel Mod          : {(plc.HasManualMode ? "Var" : "Yok")}");
            result.AppendLine($"HMI                 : {(plc.HasHmi ? "Var" : "Yok")}");
            result.AppendLine($"Seviye Kontrolü     : {(plc.HasLevel ? "Var" : "Yok")}");
            result.AppendLine($"Sıcaklık Kontrolü   : {(plc.HasTemperature ? "Var" : "Yok")}");
            result.AppendLine($"Basınç Kontrolü     : {(plc.HasPressure ? "Var" : "Yok")}");
            result.AppendLine();
        }

        private void AppendIoList(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("3. GİRİŞ / ÇIKIŞ LİSTESİ");
            result.AppendLine("------------------------");

            result.AppendLine("DİJİTAL GİRİŞLER");
            result.AppendLine("I0.0 : Start Butonu");
            result.AppendLine("I0.1 : Stop Butonu");

            int inputIndex = 2;

            if (plc.HasEmergencyStop)
                result.AppendLine($"I0.{inputIndex++} : Acil Stop");

            if (plc.HasSensor)
                result.AppendLine($"I0.{inputIndex++} : Ürün / Seviye Sensörü");

            if (plc.HasTermic)
                result.AppendLine($"I0.{inputIndex++} : Termik Arıza Kontağı");

            if (plc.HasReset)
                result.AppendLine($"I0.{inputIndex++} : Reset Butonu");

            if (plc.HasAutoMode)
                result.AppendLine($"I0.{inputIndex++} : Otomatik Mod Seçimi");

            if (plc.HasManualMode)
                result.AppendLine($"I0.{inputIndex++} : Manuel Mod Seçimi");

            if (plc.HasLevel)
            {
                result.AppendLine($"I0.{inputIndex++} : Alt Seviye Sensörü");
                result.AppendLine($"I0.{inputIndex++} : Üst Seviye Sensörü");
            }

            result.AppendLine();

            result.AppendLine("DİJİTAL ÇIKIŞLAR");

            for (int i = 1; i <= plc.MotorCount; i++)
                result.AppendLine($"Q0.{i - 1} : Motor {i}");

            for (int i = 1; i <= plc.PumpCount; i++)
                result.AppendLine($"Q1.{i - 1} : Pompa {i}");

            for (int i = 1; i <= plc.FanCount; i++)
                result.AppendLine($"Q2.{i - 1} : Fan {i}");

            for (int i = 1; i <= plc.ValveCount; i++)
                result.AppendLine($"Q4.{i - 1} : Valf {i}");

            if (plc.HasAlarm)
                result.AppendLine("Q3.0 : Alarm Lambası / Buzzer");

            result.AppendLine();

            if (plc.HasTemperature || plc.HasPressure)
            {
                result.AppendLine("ANALOG GİRİŞLER");

                if (plc.HasTemperature)
                    result.AppendLine("AI0 : Sıcaklık Sensörü");

                if (plc.HasPressure)
                    result.AppendLine("AI1 : Basınç Sensörü");

                result.AppendLine();
            }
        }

        private void AppendOperationLogic(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("4. ÇALIŞMA MANTIĞI");
            result.AppendLine("------------------");
            result.AppendLine("1. Start butonuna basıldığında sistem çalışmaya hazırlanır.");
            result.AppendLine("2. Stop butonuna basıldığında tüm çıkışlar güvenli şekilde durdurulur.");

            if (plc.HasEmergencyStop)
                result.AppendLine("3. Acil stop aktif olduğunda motor, pompa, fan ve valf çıkışları kapatılır.");

            if (plc.HasSensor)
                result.AppendLine("4. Sensör aktif olmadan ilgili çıkışlar çalıştırılmaz.");

            if (plc.HasTimer)
                result.AppendLine($"5. Timer mantığı ile çıkışlarda {plc.TimerSecond} saniyelik gecikme uygulanır.");

            if (plc.HasCounter)
                result.AppendLine($"6. Sayaç {plc.CounterLimit} adede ulaştığında sistem durdurulabilir veya alarm verilebilir.");

            if (plc.HasLevel)
                result.AppendLine("7. Seviye sensörlerine göre pompa çalıştırma/durdurma mantığı uygulanır.");

            if (plc.HasAutoMode && plc.HasManualMode)
                result.AppendLine("8. Sistem hem otomatik hem manuel modda çalışabilecek şekilde kurgulanır.");

            result.AppendLine();
        }

        private void AppendStructuredTextCode(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("5. STRUCTURED TEXT / SCL KODU");
            result.AppendLine("-----------------------------");
            result.AppendLine();

            result.AppendLine("// Kullanılan değişkenler");
            result.AppendLine("// SystemRun     : Sistem çalışma hafızası");
            result.AppendLine("// SafeCondition : Güvenlik şartı");
            result.AppendLine("// CounterValue  : Sayaç değeri");
            result.AppendLine("// ProductPulse  : Sensör tek sayım hafızası");
            result.AppendLine();

            result.AppendLine("// 1. GÜVENLİK ŞARTI");
            result.Append("SafeCondition := ");

            if (plc.HasEmergencyStop)
                result.Append("I0_2 = TRUE");
            else
                result.Append("TRUE");

            if (plc.HasTermic)
                result.Append(" AND I0_4 = FALSE");

            result.AppendLine(";");
            result.AppendLine();

            result.AppendLine("// 2. STOP / ACİL STOP / TERMİK DURDURMA");
            result.AppendLine("IF I0_1 = TRUE OR SafeCondition = FALSE THEN");
            result.AppendLine("    SystemRun := FALSE;");
            AppendAllOutputsFalse(result, plc, "    ");

            if (plc.HasAlarm)
                result.AppendLine("    Q3_0 := TRUE;");

            result.AppendLine("END_IF;");
            result.AppendLine();

            result.AppendLine("// 3. RESET BUTONU");
            if (plc.HasReset)
            {
                result.AppendLine("IF I0_5 = TRUE THEN");
                result.AppendLine("    CounterValue := 0;");
                result.AppendLine("    ProductPulse := FALSE;");

                if (plc.HasAlarm)
                    result.AppendLine("    Q3_0 := FALSE;");

                result.AppendLine("END_IF;");
            }
            else
            {
                result.AppendLine("// Reset butonu tanımlanmadığı için sayaç sıfırlama manuel eklenmelidir.");
            }

            result.AppendLine();

            result.AppendLine("// 4. START İLE SİSTEMİ BAŞLATMA");
            result.AppendLine("IF I0_0 = TRUE AND SafeCondition = TRUE THEN");

            if (plc.HasSensor)
            {
                result.AppendLine("    IF I0_3 = TRUE THEN");
                result.AppendLine("        SystemRun := TRUE;");
                result.AppendLine("    END_IF;");
            }
            else
            {
                result.AppendLine("    SystemRun := TRUE;");
            }

            result.AppendLine("END_IF;");
            result.AppendLine();

            result.AppendLine("// 5. SİSTEM ÇALIŞMA BÖLÜMÜ");
            result.AppendLine("IF SystemRun = TRUE THEN");

            if (plc.HasCounter)
            {
                result.AppendLine($"    IF CounterValue >= {plc.CounterLimit} THEN");
                result.AppendLine("        SystemRun := FALSE;");
                AppendAllOutputsFalse(result, plc, "        ");

                if (plc.HasAlarm)
                    result.AppendLine("        Q3_0 := TRUE;");

                result.AppendLine("    ELSE");
                AppendRunOutputs(result, plc, "        ");
                result.AppendLine("    END_IF;");
            }
            else
            {
                AppendRunOutputs(result, plc, "    ");
            }

            result.AppendLine("END_IF;");
            result.AppendLine();

            if (plc.HasCounter)
            {
                result.AppendLine("// 6. SAYAÇ MANTIĞI");
                result.AppendLine("IF I0_3 = TRUE AND ProductPulse = FALSE THEN");
                result.AppendLine("    CounterValue := CounterValue + 1;");
                result.AppendLine("    ProductPulse := TRUE;");
                result.AppendLine("END_IF;");
                result.AppendLine();
                result.AppendLine("IF I0_3 = FALSE THEN");
                result.AppendLine("    ProductPulse := FALSE;");
                result.AppendLine("END_IF;");
                result.AppendLine();
            }

            if (plc.HasLevel && plc.PumpCount > 0)
            {
                result.AppendLine("// 7. SEVİYE KONTROLLÜ POMPA MANTIĞI");
                result.AppendLine("IF AltSeviye = TRUE AND UstSeviye = FALSE AND SafeCondition = TRUE THEN");
                result.AppendLine("    Q1_0 := TRUE;");
                result.AppendLine("END_IF;");
                result.AppendLine();
                result.AppendLine("IF UstSeviye = TRUE THEN");
                result.AppendLine("    Q1_0 := FALSE;");
                result.AppendLine("END_IF;");
                result.AppendLine();
            }

            result.AppendLine("// NOT:");
            result.AppendLine("// Bu kod PLC mantığını göstermek için otomatik üretilmiştir.");
            result.AppendLine("// Gerçek sistemde değişken adları TIA Portal içerisindeki tag yapısına göre düzenlenmelidir.");
        }

        private void AppendRunOutputs(StringBuilder result, PlcAnalysis plc, string indent)
        {
            if (plc.MotorCount > 0)
            {
                result.AppendLine($"{indent}// Motor 1 hemen çalışır");
                result.AppendLine($"{indent}Q0_0 := TRUE;");
            }

            for (int i = 2; i <= plc.MotorCount; i++)
            {
                if (plc.HasTimer)
                {
                    int delay = plc.TimerSecond * (i - 1);

                    result.AppendLine();
                    result.AppendLine($"{indent}// Motor {i}, {delay} saniye sonra çalışır");
                    result.AppendLine($"{indent}TON_Motor{i}(IN := Q0_{i - 2}, PT := T#{delay}S);");
                    result.AppendLine($"{indent}IF TON_Motor{i}.Q = TRUE THEN");
                    result.AppendLine($"{indent}    Q0_{i - 1} := TRUE;");
                    result.AppendLine($"{indent}END_IF;");
                }
                else
                {
                    result.AppendLine($"{indent}// Motor {i}");
                    result.AppendLine($"{indent}Q0_{i - 1} := TRUE;");
                }
            }

            for (int i = 1; i <= plc.PumpCount; i++)
            {
                result.AppendLine($"{indent}// Pompa {i}");
                result.AppendLine($"{indent}Q1_{i - 1} := TRUE;");
            }

            for (int i = 1; i <= plc.FanCount; i++)
            {
                result.AppendLine($"{indent}// Fan {i}");
                result.AppendLine($"{indent}Q2_{i - 1} := TRUE;");
            }

            for (int i = 1; i <= plc.ValveCount; i++)
            {
                result.AppendLine($"{indent}// Valf {i}");
                result.AppendLine($"{indent}Q4_{i - 1} := TRUE;");
            }
        }

        private void AppendLadderExplanation(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("6. LADDER LOGIC AÇIKLAMASI");
            result.AppendLine("--------------------------");
            result.AppendLine("- Start butonu normalde açık kontak olarak kullanılır.");
            result.AppendLine("- Stop butonu normalde kapalı kontak mantığında düşünülmelidir.");

            if (plc.HasEmergencyStop)
                result.AppendLine("- Acil stop kontağı tüm çıkışlardan önce seri güvenlik kontağı olarak yerleştirilmelidir.");

            if (plc.HasSensor)
                result.AppendLine("- Sensör kontağı motor/pompa/fan çalıştırma hattına şart olarak eklenir.");

            if (plc.HasTimer)
                result.AppendLine($"- TON timer ile {plc.TimerSecond} saniyelik gecikme oluşturulur.");

            if (plc.HasCounter)
                result.AppendLine($"- Counter bloğu ürünleri sayar ve {plc.CounterLimit} değerine ulaşınca işlem yapılır.");

            if (plc.HasAlarm)
                result.AppendLine("- Alarm çıkışı arıza veya güvenlik kesmesi durumunda aktif edilir.");

            result.AppendLine();
        }

        private void AppendSafetyAnalysis(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("7. RİSK VE GÜVENLİK ANALİZİ");
            result.AppendLine("---------------------------");

            if (!plc.HasEmergencyStop)
                result.AppendLine("- KRİTİK UYARI: Acil stop tanımlanmamış. Gerçek makinede mutlaka eklenmelidir.");

            if (!plc.HasAlarm)
                result.AppendLine("- UYARI: Alarm çıkışı yok. Arıza bildirimi için alarm lambası veya buzzer önerilir.");

            if (!plc.HasSensor && plc.HasConveyor)
                result.AppendLine("- UYARI: Konveyör sisteminde ürün sensörü önerilir.");

            if (!plc.HasTermic && plc.MotorCount > 0)
                result.AppendLine("- UYARI: Motor termik koruması belirtilmemiş.");

            if (plc.MotorCount > 3)
                result.AppendLine("- NOT: Çok motorlu sistemlerde sıralı kalkış önerilir.");

            if (plc.PumpCount > 1)
                result.AppendLine("- NOT: Çok pompalı sistemlerde yedekli çalışma ve eş yaşlandırma mantığı önerilir.");

            result.AppendLine("- Gerçek PLC’ye yüklemeden önce simülasyon yapılmalıdır.");
            result.AppendLine("- AI/otomatik üretilen kod mutlaka insan kontrolünden geçmelidir.");
            result.AppendLine("- Acil stop sonrası sistem otomatik tekrar başlamamalıdır.");
            result.AppendLine();
        }

        private void AppendHmiSuggestions(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("8. HMI / PANEL ÖNERİLERİ");
            result.AppendLine("------------------------");
            result.AppendLine("- Start butonu");
            result.AppendLine("- Stop butonu");
            result.AppendLine("- Sistem çalışma durumu göstergesi");

            if (plc.HasAlarm)
                result.AppendLine("- Alarm görüntüleme ekranı");

            if (plc.HasCounter)
                result.AppendLine("- Sayaç değeri gösterimi");

            if (plc.HasTimer)
                result.AppendLine("- Timer süresi görüntüleme ve ayarlama alanı");

            if (plc.HasAutoMode || plc.HasManualMode)
                result.AppendLine("- Otomatik / Manuel mod seçimi");

            if (plc.HasTemperature)
                result.AppendLine("- Sıcaklık değeri göstergesi");

            if (plc.HasPressure)
                result.AppendLine("- Basınç değeri göstergesi");

            result.AppendLine();
        }

        private void AppendTestScenario(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("9. TEST SENARYOSU");
            result.AppendLine("-----------------");
            result.AppendLine("1. Stop butonu pasif durumdayken start butonuna basılır.");
            result.AppendLine("2. Çıkışların beklenen sırada çalıştığı kontrol edilir.");

            if (plc.HasSensor)
                result.AppendLine("3. Sensör pasifken sistemin çalışmadığı doğrulanır.");

            if (plc.HasEmergencyStop)
                result.AppendLine("4. Acil stop basıldığında tüm çıkışların kapandığı test edilir.");

            if (plc.HasTimer)
                result.AppendLine($"5. Timer süresinin {plc.TimerSecond} saniye olduğu kontrol edilir.");

            if (plc.HasCounter)
                result.AppendLine($"6. Sayaç {plc.CounterLimit} değerine ulaşınca beklenen işlemin yapıldığı test edilir.");

            result.AppendLine();
        }

        private void AppendDevelopmentSuggestions(StringBuilder result, PlcAnalysis plc)
        {
            result.AppendLine("10. GELİŞTİRME ÖNERİLERİ");
            result.AppendLine("------------------------");
            result.AppendLine("- Kod çıktısı Siemens SCL formatına daha uyumlu hale getirilebilir.");
            result.AppendLine("- Ladder görselleştirme ekranı eklenebilir.");
            result.AppendLine("- IO listesi tablo olarak gösterilebilir.");
            result.AppendLine("- Projeler veritabanına kaydedilebilir.");
            result.AppendLine("- Kullanıcı giriş sistemi eklenebilir.");
            result.AppendLine("- Gerçek OpenAI API bağlantısı aktif edilerek doğal dil desteği güçlendirilebilir.");
            result.AppendLine("- PDF veya Word rapor çıktısı eklenebilir.");
            result.AppendLine();
        }

        private void AppendAllOutputsFalse(StringBuilder result, PlcAnalysis plc)
        {
            AppendAllOutputsFalse(result, plc, "    ");
        }

        private void AppendAllOutputsFalse(StringBuilder result, PlcAnalysis plc, string indent)
        {
            for (int i = 1; i <= plc.MotorCount; i++)
                result.AppendLine($"{indent}Q0_{i - 1} := FALSE;");

            for (int i = 1; i <= plc.PumpCount; i++)
                result.AppendLine($"{indent}Q1_{i - 1} := FALSE;");

            for (int i = 1; i <= plc.FanCount; i++)
                result.AppendLine($"{indent}Q2_{i - 1} := FALSE;");

            for (int i = 1; i <= plc.ValveCount; i++)
                result.AppendLine($"{indent}Q4_{i - 1} := FALSE;");
        }

        private string GetProjectType(PlcAnalysis plc)
        {
            if (plc.HasConveyor)
                return "Konveyör Otomasyonu";

            if (plc.PumpCount > 0 || plc.HasLevel)
                return "Pompa / Seviye Kontrol Otomasyonu";

            if (plc.FanCount > 0 || plc.HasTemperature)
                return "Fan / Havalandırma Otomasyonu";

            if (plc.MotorCount > 0)
                return "Motor Kontrol Otomasyonu";

            return "Genel PLC Otomasyonu";
        }

        private bool ContainsAny(string text, params string[] words)
        {
            return words.Any(word => text.Contains(word));
        }

        private int GetTimerSecond(string scenario)
        {
            var match = Regex.Match(scenario, @"(\d+)\s*(saniye|sn|second|sec)");

            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 5;
        }

        private int GetCounterLimit(string scenario)
        {
            var match = Regex.Match(scenario, @"(\d+)\s*(adet|ürün|parça)");

            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 10;
        }

        private int GetNumberBeforeWord(string scenario, string word, int defaultValue)
        {
            var match = Regex.Match(scenario, @"(\d+)\s*" + word);

            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return defaultValue;
        }

        [HttpPost]
        public IActionResult DownloadTxt(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "PLC raporu oluşturulamadı.";
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(result);

            return File(fileBytes, "text/plain", "PLC_Copilot_Raporu.txt");
        }
        [HttpPost]
        public IActionResult DownloadPdf(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "PLC raporu oluşturulamadı.";
            }

            QuestPDF.Settings.License = LicenseType.Community;

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("PLC COPILOT AI RAPORU")
                        .FontSize(24)
                        .Bold();

                    page.Content()
                        .PaddingVertical(20)
                        .Text(result)
                        .FontSize(12);

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("PLC Copilot AI");
                        });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "PLC_Raporu.pdf");
        }

        private class PlcAnalysis
        {
            public string Scenario { get; set; } = "";

            public int MotorCount { get; set; }
            public int PumpCount { get; set; }
            public int FanCount { get; set; }
            public int ValveCount { get; set; }
            public int SensorCount { get; set; }

            public int TimerSecond { get; set; }
            public int CounterLimit { get; set; }

            public bool HasConveyor { get; set; }
            public bool HasSensor { get; set; }
            public bool HasEmergencyStop { get; set; }
            public bool HasAlarm { get; set; }
            public bool HasCounter { get; set; }
            public bool HasStartStop { get; set; }
            public bool HasTimer { get; set; }
            public bool HasHmi { get; set; }
            public bool HasAutoMode { get; set; }
            public bool HasManualMode { get; set; }
            public bool HasLevel { get; set; }
            public bool HasTemperature { get; set; }
            public bool HasPressure { get; set; }
            public bool HasTermic { get; set; }
            public bool HasReset { get; set; }
        }
    }
}