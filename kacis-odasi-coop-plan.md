# Kaçış Odası Co-op (1-4 Kişi Online) — Proje Yol Haritası

## ÖNEMLİ NOT — Nasıl Yardım Edilmeli

Ben (bu projeyi yürüten kişi) Unity ve kodlama konusunda **hiçbir deneyime sahip değilim**. Daha önce basit oyunlar (top toplama, araba kaçırma) yaptım ama bunları da **tamamen adım adım rehberlik alarak** yaptım — kendi başıma Unity Editor'de nereye tıklayacağımı bilmiyorum.

Bu yüzden bana yardım eden herkesin (Claude Code dahil) şu şekilde ilerlemesi gerekiyor:
- Her adımı **tek tek, sırayla** anlat — "şunu şöyle yap, sonra bana ne olduğunu söyle" şeklinde, birden fazla adımı üst üste yığmadan
- Unity Editor'de **tam olarak nereye tıklayacağımı** söyle (hangi menü, hangi buton, hangi panel)
- Kod verirken, o kodu **hangi dosyaya, nasıl** ekleyeceğimi de anlat
- Bir şey yaptıktan sonra ne görmem gerektiğini söyle ki doğru yapıp yapmadığımı anlayabileyim
- Teknik terimleri kullanırken kısaca ne anlama geldiğini açıkla
- Sabırlı ol — muhtemelen çok fazla "bunu bulamadım", "böyle bir şey çıkmadı" gibi geri bildirim vereceğim, bunlar normal

## Genel Bakış

**Oyun:** 1-4 oyuncunun aynı anda, aynı odada, birlikte bulmaca çözerek kaçmaya çalıştığı online co-op oyun.

**Temel döngü:** Oyuncular odaya girer → etrafta gezinir, objelerle etkileşime girer → bazı bulmacalar TEK oyuncuyla çözülür, bazıları İKİ+ oyuncunun AYNI ANDA farklı yerlerde bir şey yapmasını gerektirir → süre bitmeden kapıyı/çıkışı bulup kaçarlar.

**Neden bu tür kolay (göreceli olarak):** Hızlı refleks/anlık senkronizasyon gerektirmiyor. Bir oyuncunun pozisyonu diğer ekranda 100-200ms gecikmeli görünse bile oyun deneyimi bozulmaz — bu da networking tarafını FPS/aksiyon oyunlarına göre çok daha affedici yapıyor.

## Teknoloji Yığını

- **Unity** (mevcut sürüm, 6000.5.x)
- **Unity Netcode for GameObjects (NGO)** — ücretsiz, resmi paket, oyuncu senkronizasyonu için
- **Unity Relay** — oyuncuların birbirine bağlanması için (kendi sunucu kiralamana gerek kalmadan, ücretsiz tier yeterli)
- **Unity Lobby** — oyuncuların bir "oda kodu" ile birbirini bulması için
- **Unity Authentication (Anonymous)** — Relay/Lobby'nin çalışması için gereken temel kimlik doğrulama

---

## FAZ 0 — Proje Kurulumu
- [x] Yeni Unity projesi oluştur (3D ya da 2D — önerim: basit 3D, oda hissi daha kolay verir, ama 2D top-down da olur)
- [x] Git + GitHub reposu kur (önceki projelerdeki gibi)
- [x] Package Manager'dan şu paketleri yükle:
  - [x] Netcode for GameObjects
  - [x] Unity Transport
  - [x] Relay/Lobby (birleşik "Multiplayer Services" paketi olarak — Relay paketi deprecated olduğu için)
  - [x] Authentication
- [x] Unity Cloud / Unity Gaming Services (UGS) hesabını projeye bağla (Services penceresinden)

**Milestone:** Paketler hatasız import oluyor, Services paneli bağlı görünüyor.

---

## FAZ 1 — Networking Temelleri (EN KRİTİK FAZ)
- [x] Boş bir sahnede `NetworkManager` objesi oluştur
- [x] Basit bir "Player" prefab'ı yap (kapsül/küp, `NetworkObject` + `NetworkTransform` component'leriyle — owner-authoritative `ClientNetworkTransform` kullanıldı)
- [x] Host/Client başlatma UI'ı: "Host Olarak Başlat" / "Katıl" butonları
- [x] Relay ile bir "oda kodu" üretme (host için) ve kodla katılma (client için) akışını kur
- [x] Test: İki farklı Unity Editor penceresi (ya da bir build + bir editor) açıp, biri host, diğeri client olarak bağlanabiliyor mu kontrol et — Windows Editor/build + Mac Editor arasında da gerçek Relay testi yapıldı
- [x] İki oyuncunun hareketinin birbirinde göründüğünü doğrula

**Milestone:** İki ayrı bilgisayar/pencere, aynı oda koduyla bağlanıp, ikisi de aynı sahnede birbirinin hareketini görebiliyor. **Bu noktaya gelmek, projenin en zor %40'ını geçmek demek.**

**Beklenen zorluklar:** NetworkTransform senkronizasyon gecikmeleri, "kimin client kimin host olduğu" karışıklığı, Relay bağlantı hataları (genelde Unity Dashboard'da proje ayarı eksikliğinden). Bunlar normal, sabırla debug edilir.

---

## FAZ 2 — Oda ve Ortam
- [x] Basit bir oda modeli/sahnesi kur (4 duvar, zemin, birkaç obje — detaylı sanat şimdilik önemli değil)
- [x] Kamera ayarları (FPS/first-person seçildi — CameraHolder + fare ile bakış)
- [x] Oyuncu karakterine basit bir görsel (capsule yeterli, sonra değiştiririz)
- [x] Işıklandırma (atmosfer için, basit bile olsa fark yaratır) — varsayılan Directional Light + Global Volume yeterli görüldü

**Milestone:** Oda içinde 2-4 oyuncu dolaşabiliyor, çarpışma/duvar sınırları çalışıyor.

---

## FAZ 3 — Etkileşim Sistemi
- [x] "Etkileşime girilebilir obje" için temel bir script (örn. `Interactable.cs`)
- [x] Oyuncunun yakınındaki objeyle etkileşime girmesi (E tuşu ya da tıklama)
- [x] Etkileşimin **networked** olması — yani bir oyuncunun bir objeyi açması, diğer oyuncuların ekranında da görünmesi (Rpc + NetworkVariable ile yapıldı)
- [x] Basit bir örnek: bir çekmeceyi aç, içinden bir anahtar/ipucu çıksın, herkes görsün

**Milestone:** Bir oyuncunun yaptığı etkileşim, diğer oyuncunun ekranında da anında yansıyor.

---

## FAZ 4 — Co-op Bulmacalar (oyunun kalbi)
- [ ] **Bulmaca 1 (basit, tek kişilik):** Bir şifre bulup bir kilide girme
- [ ] **Bulmaca 2 (co-op, iki kişilik):** İki farklı düğmeye AYNI ANDA iki farklı oyuncunun basması gerekiyor (bu, "networked event senkronizasyonu" öğretir)
- [ ] **Bulmaca 3 (co-op, bilgi paylaşımı):** Bir oyuncu bir ipucunu görür (örn. bir duvarda yazı), ama şifreyi girmesi gereken obje odanın başka bir yerinde, diğer oyuncunun yanında — oyuncular birbirine (oyun içi chat ya da sesli konuşarak) bilgi aktarmalı
- [ ] Bulmacaların çözülme durumunun (kim neyi açtı, hangi kilit açıldı) TÜM oyunculara senkronize olduğundan emin ol

**Milestone:** En az 3 farklı bulmaca türü çalışıyor, ikisi gerçek co-op gerektiriyor (yalnız oynanamıyor).

---

## FAZ 5 — Oyun Döngüsü
- [ ] Geri sayım süresi (örn. 10 dakika), tüm oyunculara senkronize
- [ ] Kazanma durumu: oda çıkışını bulup kaçınca "Kaçtınız!" ekranı
- [ ] Kaybetme durumu: süre bitince "Süre Doldu" ekranı
- [ ] Lobby'ye/ana menüye dönüş akışı

**Milestone:** Baştan sona oynanabilir bir tur: lobiye katıl → oda içinde bulmacaları çöz → kazan/kaybet → tekrar oyna.

---

## FAZ 6 — Cila ve Test
- [ ] Ses efektleri (etkileşim, başarı, hata sesleri)
- [ ] Basit ana menü + oda kodu paylaşma ekranı (arkadaşına kodu WhatsApp'tan atman kolay olsun diye)
- [ ] Gerçek arkadaşlarınla 2-4 kişilik test oturumu yap
- [ ] Test sırasında çıkan networking bug'larını (senkronizasyon kopması, biri bağlanamıyor gibi) not al, sırayla çöz

**Milestone:** Gerçek arkadaşların, farklı bilgisayarlardan/ağlardan bağlanıp sorunsuz bir tur oynayabiliyor.

---

## FAZ 7 (İleri seviye, opsiyonel) — Yayın
- [ ] Build alıp arkadaşlarına .exe/.app olarak dağıtma
- [ ] Steam'de yayınlamak istersen: Steamworks + P2P networking uyumluluğu (bu ayrı bir araştırma gerektirir)
- [ ] Daha fazla oda/bölüm ekleme

---

## Riskler — Gerçekçi Beklenti

- **Faz 1, projenin en can sıkıcı fazı olacak.** Görünürde "hiçbir şey olmuyormuş" gibi hissedebilirsin ama arka planda en kritik altyapı kuruluyor. Sabırlı ol.
- Networking hataları genelde **belirsiz/soyut hata mesajları** verir (Unity script hatası gibi net değildir). Bu yüzden Claude Code ile çalışırken, hatayı olduğu gibi (tam mesaj + hangi tarafta - host mu client mı - olduğu) paylaşmak çok önemli.
- **İki bilgisayarda aynı anda test etmek şart** — tek bilgisayarda "host + client" simüle edilebilir (iki ayrı build/editor penceresi açarak) ama gerçek ağ gecikmesini görmek için en az bir kez farklı ağlardan (örn. sen + bir arkadaşın, farklı evlerden) test etmen lazım.

## Claude Code ile Çalışma Önerisi

1. Bu dosyayı projenin kök dizinine `PLAN.md` olarak koy
2. Her fazı ayrı bir "oturum" olarak ele al — bir fazı bitirmeden diğerine geçme
3. Hata aldığında Claude Code'a: (a) tam hata mesajını, (b) hangi tarafta olduğunu (host/client/ikisi), (c) o anki ilgili script'i birlikte ver
4. Her milestone'a ulaştığında git commit at — bu, bir şey bozulursa geri dönebileceğin güvenli noktalar oluşturur
