
🎧 Bepop Müzik Uygulaması (AI Destekli Müzik Öneri Platformu)
Bepop, kullanıcıların müzik zevklerini analiz eden, dinleme geçmişlerine göre yapay zekâ (AI) ve makine öğrenmesi (ML) destekli şarkı önerileri sunan modern bir web uygulamasıdır. ASP.NET Core 9, PostgreSQL, ML.NET ve OpenAI GPT-4o-mini altyapısıyla geliştirilmiştir. Uygulama, kullanıcıların dinleme davranışlarını analiz eder, tür bazlı eğilimleri belirler ve kişiye özel müzik listeleri oluşturur. Bepop’un en güçlü yanı, yalnızca puan bazlı öneriler sunmakla kalmayıp, AI tarafından Türkçe doğal dilde yazılmış müzik analizlerini de kullanıcıya sunmasıdır.

Bu proje Onion Architecture (Soğan Mimarisi) prensiplerine uygun olarak geliştirilmiştir. Katmanlar arası bağımlılıklar merkeze (Domain’e) doğru akmakta, her katman sadece bir iç halkaya bağımlı olacak şekilde yapılandırılmıştır. Bu sayede sistem yüksek oranda modüler, test edilebilir ve genişletilebilir hale gelmiştir.

🧩 Mimari Katmanlar:
1️⃣ Domain Katmanı: Uygulamanın kalbidir. Entity, DTO, Value Object ve temel iş kurallarını barındırır.
2️⃣ Application Katmanı: Servisler, yapay zekâ (AI) entegrasyonları, ML.NET modelleri ve iş mantıkları burada bulunur.
3️⃣ Infrastructure Katmanı: Veritabanı işlemleri, EF Core konfigürasyonları, repository yapıları, OpenAI servisleri ve ML model dosyaları bu katmanda yer alır.
4️⃣ WebUI Katmanı: Razor View’lar, Controller’lar, kullanıcı arayüzü, API tüketimi ve görsel bileşenler burada çalışır.

Bepop, kullanıcıların dinleme geçmişlerini UserPlays tablosunda saklar. Bu verilerden RecommendationModel.PredictScore() fonksiyonu aracılığıyla şarkılara puanlama yapılır. Ardından OpenAiService.ExplainAsync() metodu devreye girer ve kullanıcıya Türkçe açıklamalı müzik önerisi sunar. Örneğin; “Son zamanlarda enerjik pop şarkılar dinlediğini fark ettim. Ruh haline uygun bazı yeni parçalar buldum 🎶” gibi samimi AI mesajları oluşturulur.

Veritabanı PostgreSQL üzerinde çalışır ve Entity Framework Core kullanılarak yönetilir.
Tablolar şunlardır:

Users: Kullanıcı bilgileri (Email, PasswordHash, MembershipLevel, CreatedAt)

Songs: Şarkı bilgileri (Title, Artist, Genre, Level, DurationSec, CoverUrl, CreatedAt)

Artists: Sanatçılar (FirstName, LastName, Country, PhotoUrl, Genres)

UserPlays: Kullanıcının dinleme geçmişi (UserId, SongId, PlayedAt)

Recommendations: Yapay zekâ öneri kayıtları

AI öneri sistemi iki aşamalı çalışır:
1️⃣ ML.NET modeli kullanıcı-şarkı etkileşimlerine göre öneri puanları üretir.
2️⃣ OpenAI modeli bu verileri yorumlayarak kullanıcıya doğal dilde kişisel analiz raporu oluşturur.

🔒 Güvenlik:
JWT tabanlı kimlik doğrulama, ASP.NET Session yönetimi, HTTPS bağlantısı, Cookie güvenlik politikaları (HttpOnly, SameSite) ve EF Core SQL Injection koruması entegre edilmiştir.

🎨 Arayüz:
Bootstrap 5, jQuery ve modern responsive tasarım yaklaşımıyla geliştirilmiştir.
Discover sayfasında AI analizi ve önerilen şarkılar, Genres sayfasında tür bazlı AI önerileri, Artists sayfasında sanatçı profilleri, All Songs sayfasında sistemdeki tüm şarkılar listelenir.
Ayrıca CreatedAt sütununa göre sıralanmış “En Son Eklenen Şarkılar” bölümü de bulunur.

⚙️ Teknik Özellikler:

Backend: ASP.NET Core 9 (C#)

Frontend: Bootstrap 5, jQuery

ORM: Entity Framework Core

Veritabanı: PostgreSQL 16

AI Servisleri: OpenAI GPT-4o-mini + ML.NET

Oturum Yönetimi: JWT + ASP.NET Session

Raporlama: iTextSharp (PDF) & ClosedXML (Excel)

Port: HTTPS (7273)

💾 Kurulum ve Çalıştırma:
1️⃣ Bilgisayarında .NET 9 SDK, PostgreSQL 16 ve Visual Studio 2022 kurulu olmalıdır.
2️⃣ Veritabanını oluşturmak için terminalde şu komutu çalıştır:
dotnet ef database update
3️⃣ appsettings.json içine PostgreSQL bağlantı bilgisini ekle:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MusicAppDb;Username=postgres;Password=12345"
}
4️⃣ OpenAI API anahtarını tanımla:
"OpenAI": {
  "ApiKey": "YOUR_API_KEY",
  "Model": "gpt-4o-mini"
}
5️⃣ Projeyi başlatmak için:
dotnet run --project MusicApp.WebUI
6️⃣ Uygulama tarayıcıda şu adreste çalışacaktır:
🔗 https://localhost:7273

💡 Kullanım:
Kullanıcı kayıt olup giriş yaptıktan sonra müzik dinledikçe dinleme geçmişi otomatik kaydedilir.
AI, geçmişe göre kullanıcı profili oluşturur, dinleme eğilimlerini analiz eder ve “AI Tarafından Önerilen Şarkılar” bölümünde açıklamalı öneriler sunar.
Ayrıca tür bazlı öneriler, sanatçı sayfaları ve en son eklenen şarkılar dinamik olarak güncellenir.

📊 Performans ve Optimizasyon:
Asenkron sorgular, EF Core AsNoTracking kullanımı, ML model cacheleme, OpenAI sonuçlarının veritabanına kaydedilmesi, lazy loading devre dışı optimizasyonları ve loglama sistemi bulunmaktadır.

🔐 Lisans:
Proje MIT Lisansı ile yayınlanmıştır. Geliştiriciler kodu serbestçe kullanabilir, düzenleyebilir ve genişletebilir.

👨‍💻 Geliştirici:
Ömer Faruk YAŞAR
GitHub: github.com/omeryasar745
E-posta: omeryasar745@gmail.com
🌟 “Onion mimarisi, yapay zekâ ve modern .NET teknolojileriyle kişiselleştirilmiş müzik deneyimi — Bepop ile ritmini keşfet!”
<img width="1879" height="869" alt="MusicApp-MainPage" src="https://github.com/user-attachments/assets/3dac69f8-55b8-45b7-9ead-2551d8f18bbc" />
<img width="1885" height="755" alt="MusicApp-Genres-1" src="https://github.com/user-attachments/assets/67efdff8-d877-480a-acb0-5a538452f213" />
<img width="1885" height="877" alt="MusicApp-Genres2" src="https://github.com/user-attachments/assets/4824c269-5f02-4eb6-8baa-53cd561f85b1" />
<img width="1852" height="845" alt="MusicApp-Discover-1" src="https://github.com/user-attachments/assets/61610b30-f2a4-4563-8c46-631638e2040c" />
<img width="1869" height="874" alt="MusicApp-Discover2" src="https://github.com/user-attachments/assets/e0f945e3-d699-451e-bd0b-8f80f3706070" />
<img width="1853" height="849" alt="MusicApp-Charts" src="https://github.com/user-attachments/assets/770bbfc2-9098-426d-903b-f310be05537c" />
<img width="1865" height="792" alt="MusicApp-Artists" src="https://github.com/user-attachments/assets/f3e25b8b-320b-4ca3-8955-cc380f032eca" />
<img width="1895" height="829" alt="MusicApp-SignUp" src="https://github.com/user-attachments/assets/d9dfa5c9-0989-427a-9493-7db3dd6221a4" />
<img width="1903" height="870" alt="MusipApp-SignIn" src="https://github.com/user-attachments/assets/9e25978b-e114-47f3-b90e-410ad71e996e" />














