
# Ödev: Berber App

## Açıklama
Bu proje, MVC tabanlı ASP.NET ile geliştirilmiş bir berber randevu uygulamasıdır. Uygulama, berber randevularını yönetmek, kazanç istatistiklerini izlemek ve kullanıcı deneyimini artırmak amacıyla çeşitli özellikler sunmaktadır.

## Yapılanlar
| Özellik                               | Durum    |
|---------------------------------------|----------|
| Randevu Sistemi                      | Tamam    |
| Fatura Oluşturma                     | Tamam    |
| Kazanç İstatistiği İzleme            | Tamam    |
| Slider Yönetimi                      | Tamam    |
| AI ile Saç Simülasyonu               | Tamam    |
| API Çıkarma (LINQ Kullanarak)       | Tamam    |

## Projeyi Başlatma

1. **Projeyi Klonlayın**:
   ```bash
   git clone <repository-url>
   ```

2. **Migrate İşlemi**:
   Veritabanını oluşturmak için gerekli migrasyonları uygulayın:
   ```bash
   cd <project-directory>
   dotnet ef database update
   ```

3. **Admin Girişi**:
   - Admin paneline erişmek için `/admin` adresine gidin.
   - Başlangıç bilgileri:
     - **Kullanıcı Adı**: `b221210029@sakarya.edu.tr`
     - **Şifre**: `sau`

## Admin Paneli Özellikleri
Admin paneli, uygulamanın yönetim işlevlerini kolaylaştırmak için tasarlanmıştır. Aşağıdaki özellikleri içerir:

- **Dashboard**: Uygulama istatistikleri, kazanç bilgileri ve genel durum görüntülenir.
- **Randevu Yönetimi**: Tüm randevuların görüntülenmesi ve durumlarının güncellenmesi. Randevu durumunu "complete" olarak ayarlamak, fatura oluşturma sürecini başlatır.
- **Fatura Yönetimi**: Oluşturulan faturaların görüntülenmesi, yönetilmesi ve ödemelerin takibi.
- **Çalışan Yönetimi**: Çalışanların eklenmesi, güncellenmesi ve silinmesi. Çalışanlara beceriler atanabilir ve temel maaş bilgileri yönetilebilir.
- **Beceri Yönetimi**: Sunulan becerilerin eklenmesi, güncellenmesi ve silinmesi. Her beceri için fiyat, bonus ve süre bilgileri belirlenebilir.
- **Slider Yönetimi**: Ana sayfadaki slider görsellerinin eklenmesi, güncellenmesi ve silinmesi.
- **Genel Ayarlar Yönetimi**: Uygulamanın genel ayarlarının güncellenmesi, logo, SEO bilgileri ve ana sayfa ayarları.
- **Mevcut Zaman Yönetimi**: Çalışanların haftanın her günü için açılış ve kapanış saatlerinin belirlenmesi.

## Lisans
Bu proje MIT lisansı ile lisanslanmıştır.
