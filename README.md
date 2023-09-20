# Epic Merge projesindeki implementasyonu yapılan sistemler hakkındaki bilgiye aşağıdaki maddelerden ulaşabilirsiniz.

- Oyundaki ana mekanik olan merge işlemindeki kontrollerin performanslı şekilde yapılabilmesi için Graph tercih edildi. Bu tercih, yapılacak işlemlerin haritanın dikdörtgen  grid olmadığı durumlara da uygun olması için yapıldı.

- Generic Graph implementasyonu yapıldı.  
	- Graph<T, TF> içerisinde Node<T, TF> node listesi tutulmaktadır. 
	- T Node içerisindeki Value tipini, TF ise her bir Node içerisinde tutulan Neighbor node tipini temsil etmektedir.	
	- Generic Node class'ından kalıtım alınarak TileNode oluşturuldu. (T : TileObjectValue, TF : TileNode)
	- Generic Graph class'ından kalıtım alınarak da TileGraph oluşturuldu.
		- TileGraph daki TileNode'lar bitişin olacağından, komşu nodelar arasındaki mesafenin node genişliği(veya nodeDistance parametresi) kadar olacağı varsayımı kullanılarak tüm TileNode'lar için Neighbors listesi koddan otomatik olarak dolduruldu.
		- Listenin otomatik doldurulması işlemi için TileGraphExtensions extension class ve içerisine FindEdgedWithNodeDistance fonksiyonu oluşturuldu. Böylece neighbor listesi otomatik doldurulması istenen graph için ilgili fonksiyonun obje üzerinden çağrılabilmesi sağlandı.

- Merge işlemlerini yönetmesi için MergeHelper static class implemente edildi.

- Tile Node içerisinde üzerinde bulunan objenin id ve level bilgisini tutan TileObjectValue değeri tutulmaktadır. Merge durumunun sağlanması için yan yana bulunan en az 3 objenin (veya 2 yan yana obje ve üzerine sürüklenen objenin) id ve level değerlerinin aynı olması gerekmektedir. 
	- Merge için gereken obje sayısı GameConfiguration'dan parametrik olarak ayarlanmıştır ve kolayca değiştirilebilmektedir.

- Merge şartının sağlanıp sağlanmadığının kontrolü için merge işleminin başlatıldığı node üzerinden BFS(Breadth First Search) algoritması uygulanmakta ve TileNode içerisindeki TileObjectValue değerleri aynı olan komşu objeler bulunmaktadır.

- Oyundaki objelerin birbiriyle haberleşmesinde observer pattern kullanılmaktadır. Event Service class'ı içerisinde tanımlanan eventler gerekli objeler tarafından dinlenmekte ve invokelanmaktadır.

- TileObjectlerin id ve level a göre bilgilerinin girilmesi için TilObjectCollection oluşturulmuştur. 
	- TileObjectManager, ilgili collectionun load işlemini yaptıktan sonra objelerin datasına O(1) zamanda erişilmesi için dictioanary ile id ve TileObjectData eşlemesi yapılmaktadır.

- Oyundaki objenin sürüklenmesi, merge edilebilir olması gibi durumlarda kullacıyıya feedback verilmesi için Indicator System implementasyonu yapılmıştır.
	- IIndicatorController interface ve BaseIndicatorController class tanımlandı.
	- IIndicator interface ve BaseIndicator class tanımlandı.
	- BaseIndicatorController içerisinde List<BaseIndicator> listesi bulunmaktadır (SerializeField ile doldurulduğu için IIndicator listesi tutulmadı).
	- Tile Objectlerin merge edilebildiği durumunda kullanıcıya feedback verilmesi icin BaseIndicatorController'dan kalıtım alınarak TileObjectMergeableIndicatorController class oluşturuldu. 
		- Merge edilebildiği durumda Tile Object'in altında bir obje açılması ve objenin ping pong salınım hareketleri için BaseIndicator class dan kalıtım alınanak ObjectPinPongIndicator ve ObjectShowIndicator classları oluşturuldu. Tile Object üzerine atılan bu componentler  TileObjectMergeableIndicatorController içerisindeki listeye eklendi ve objeler merge edilebilir olduğunda bu controller üzerinden indicator show ve hide işlemleri yapıldı.


- TileObject ve TileNode arasındaki detection'ın algılanması ve yönetilmesi için Detection System implementasyonu yapıldı.
	- IObjectDetector, IObjectDetectionHandler ve ITileNodeDetectionHandler interfaceleri tanımlandı.
	- TileObject kendi içerisinde IObjectDetector ve IObjectDetectionHandler interfacelerini implmente eden objeleri içermektedir.
	- TileObject, IObjectDetecter içerisindeki OnEnteredGameObject ve OnExitedGameObject eventlerini dinleyerek detection olduğunnda ilgili game object bilgisiyle birlikte durumdan haberdar olmaktadır. Ek olarak, Tile Object istediği durumlarda IObjectDetector içerisindeki IsDetectionActive parametresini set ederek detection durumunu kontrol edebilmektedir. 
	- Tile Object, object detection bilgisi geldiğinde ilgili objeyi IObjectDetectionHandler'a göndererek detectionun yönetilmesini sağlamaktadir.
	- IObjectDetectionHandler implementasyonunu yapan ITilObjectDetectionHandler class objesi detection olan obje içerisinden 
	- TileNodeObjectController objesi, kendi içerisinde ITileObjectDetectionHandler implementasyonunu yapan TileNodeObjectDetectionHandler objesini tutmakta ve TileObject'dekine benzer şekilde içerisindeki eventleri dinlemektedir.
	- ITileNodeDetectionHandler componentini çekerek detection türüne göre ObjectEnterTileArea veya ObjectExitTileArea fonksiyonlarını ilgili TileObject parametlerisi ile birlikte çağırmaktadır.
	- Detectiondan haberdar olan TileNode ilgili TileObject için gerekli işlemleri yapmaktadır.

- Tile Object Move işlemlerini yapılması için MoveSystem implementasyonu yapıldı ve hareket edilmesi istendiğinde TileObject içerisindeki IMoveController üzerinden ilgili parametreler ile Move fonksiyonu çağrıldı.

- Oyundaki TileGraphların kolayca oluşturulabilmesi için Tile Graph Generator class oluşturuldu
	- Parametre olarak haritanın boyutları, TileNodeType, TileObjetType, Node Distance ve haritadaki bazı tile node bölgelerinin random olarak boş olması için bir parametre alındı.
	- Alınan parametleler kullanılarak graph oluşturuldu ve TileNode'ların üzerine random oluşturulan TileObject'ler eklendi.
 	- Tüm TilGraphGenerator objelerin OnEnable ve OnDisable içerisinde kendilerini TileGraphGeneratorManager içerisindeki listeye ekleyip çıkarmaktadır. Böylece TileGraphGeneratorManager o an ulaşılabilir olan generatorları bilmektedir.
 	- Game Starter içerisinde oyun başlangıcında TileGraphGeneratorManager içerisindeki listede olan tüm generatorlar için Crate Graph fonksiyonu çağrılarak oyun başlangıcında harita oluşturuldu.
	- Settings paneldeki Recreate graph butonlarına tıklandığında ilgil graph için generate işlemi tekrarlandı.

- Oyundaki panellerin resource dan yüklenmesi, istenen yerden panel type değeri ile gösterilip gizlenebilmesi için PanelSystem implementasyonu yapıldı.
	- İçerisinde settings panelin açılmasını sağlayan Gameplay panel oluşturuldu.
	- Settings panel de her bir graph'ın runtime da yeniden oluşturulmasını sağlayan recreate graph butonları eklendi. 


Optimizasyon için yapılanlar : 

- Objeler arasındaki kontrol işlemi sırasındaki gezmelerin daha performanslı olması için Graph kullanımı.

- MonoBehovior içerisinde Update event fonksiyonlarının çağrılmasının maliyetini azaltmak için UpdateManager paketinin import edilmesi ve tüm Update fonksiyonunda yapılacak çağrıların UpdateManager içerisindeki tek Update event fonksiyonundan çağrılması sağlandı.

- Oyundaki tris ve vertex sayısının azaltılması amacıyla tile object görüntüsü için mesh yerine SpriteRenderer kullanıldı.

- Oyundaki batch sayısının azaltılması için kullanılan yöntemler :
	- Ortak mesh kullanan objeler için gpu instance.
	- Tree spritelarının tek texture içerisinde toplanıp multiple sprite mode kullanılması.
	- UI daki spritelar için SpriteAtlas kullanılması.
