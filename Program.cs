using System;
using System.Linq;
using KARTABUS.Data;
using KARTABUS.Models;
using Microsoft.EntityFrameworkCore;

namespace KARTABUS
{
    internal class Program
    {
        static int? selectedScheduleId = null;
        static int? selectedSeatId = null;

        static void Main(string[] args)
        {
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureCreated();

                if (!db.Admins.Any())
                {
                    db.Admins.Add(new Admin
                    {
                        Name = "Administrator",
                        Username = "admin",
                        Password = "admin123",
                        Position = "Operator"
                    });

                    db.SaveChanges();
                }
            }

            MainMenu();
        }

        // ================= MENU UTAMA =================

        static void MainMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Header();

                Console.WriteLine("Selamat Datang di KARTABUS!");
                Console.WriteLine("Solusi Pemesanan Tiket Bus");
                Console.WriteLine();
                Console.WriteLine("[1] Registrasi Pengguna");
                Console.WriteLine("[2] Login");
                Console.WriteLine("[3] Keluar");
                Console.WriteLine();
                Console.Write("Pilih Menu : ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Register();
                        break;

                    case "2":
                        Login();
                        break;

                    case "3":
                        running = false;
                        Console.WriteLine("\nTerima kasih telah menggunakan KARTABUS!");
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void Header()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("                    KARTABUS");
            Console.WriteLine("          BUS TICKET MANAGEMENT SYSTEM");
            Console.WriteLine("==================================================");
            Console.WriteLine();
        }

        // ================= REGISTRASI =================

        static void Register()
        {
            Console.Clear();
            Header();

            Console.WriteLine("              REGISTRASI PENGGUNA");
            Console.WriteLine();

            Console.Write("Nama Lengkap : ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Username     : ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Password     : ");
            string password = Console.ReadLine() ?? "";

            Console.Write("Nomor HP     : ");
            string phone = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone))
            {
                Message("Semua data wajib diisi!");
                return;
            }

            using (var db = new DatabaseContext())
            {
                if (db.Users.Any(u => u.Username == username))
                {
                    Message("Username sudah digunakan!");
                    return;
                }

                db.Passengers.Add(new Passenger
                {
                    Name = name,
                    Username = username,
                    Password = password,
                    PhoneNumber = phone
                });

                db.SaveChanges();
            }

            Message("Registrasi berhasil! Silakan login.");
        }

        // ================= LOGIN =================

        static void Login()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                  LOGIN AKUN");
            Console.WriteLine();

            Console.Write("Username : ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Password : ");
            string password = Console.ReadLine() ?? "";

            using (var db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u =>
                    u.Username == username &&
                    u.Password == password);

                if (user == null)
                {
                    Message("Username atau password salah!");
                    return;
                }

                if (user is Admin)
                {
                    AdminMenu(user.Name);
                }
                else if (user is Passenger)
                {
                    selectedScheduleId = null;
                    selectedSeatId = null;

                    PassengerMenu(user.Name, user.UserId);
                }
            }
        }

        // ================= MENU PENUMPANG =================

        static void PassengerMenu(string name, int userId)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                MENU PENUMPANG");
                Console.WriteLine($"Selamat Datang, {name}!");
                Console.WriteLine();
                Console.WriteLine("[1] Pencarian Bus");
                Console.WriteLine("[2] Daftar Jadwal Bus");
                Console.WriteLine("[3] Detail Perjalanan");
                Console.WriteLine("[4] Pemilihan Kursi");
                Console.WriteLine("[5] Pemesanan Tiket");
                Console.WriteLine("[6] Simulasi Pembayaran");
                Console.WriteLine("[7] E-Tiket");
                Console.WriteLine("[8] Riwayat Pemesanan");
                Console.WriteLine("[9] Logout");
                Console.WriteLine();
                Console.Write("Pilih Menu : ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                        PassengerFeature(choice, userId);
                        break;

                    case "9":
                        logout = true;
                        selectedScheduleId = null;
                        selectedSeatId = null;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        // ================= MENU ADMIN =================

        static void AdminMenu(string name)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                  MENU ADMIN");
                Console.WriteLine($"Selamat Datang, {name}!");
                Console.WriteLine();
                Console.WriteLine("[1] Kelola Data Bus");
                Console.WriteLine("[2] Kelola Rute");
                Console.WriteLine("[3] Kelola Jadwal");
                Console.WriteLine("[4] Kelola Harga");
                Console.WriteLine("[5] Kelola Kursi");
                Console.WriteLine("[6] Kelola Pemesanan");
                Console.WriteLine("[7] Kelola Pengguna");
                Console.WriteLine("[8] Logout");
                Console.WriteLine();
                Console.Write("Pilih Menu : ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        AdminFeature(choice);
                        break;

                    case "8":
                        logout = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        // =====================================================
        //                    FITUR PENUMPANG
        // =====================================================

        static void PassengerFeature(string choice, int userId)
        {
            switch (choice)
            {
                case "1":
                    PencarianBus();
                    break;

                case "2":
                    DaftarJadwalBus();
                    break;

                case "3":
                    DetailPerjalanan();
                    break;

                case "4":
                    PemilihanKursi();
                    break;

                case "5":
                    PemesananTiket(userId);
                    break;

                case "6":
                    SimulasiPembayaran(userId);
                    break;

                case "7":
                    ETiket(userId);
                    break;

                case "8":
                    RiwayatPemesanan(userId);
                    break;
            }
        }

        // 1. PENCARIAN BUS

        static void PencarianBus()
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === PENCARIAN BUS ===");
            Console.WriteLine();

            Console.Write("Masukkan nama kota tujuan : ");
            string tujuan = Console.ReadLine() ?? "";

            using var db = new DatabaseContext();

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .ToList()
                .Where(s => s.Route != null &&
                    s.Route.Destination.Contains(
                        tujuan,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine("\nHasil Pencarian :\n");

            if (jadwal.Count == 0)
            {
                Console.WriteLine("Bus dengan tujuan tersebut belum tersedia.");
            }
            else
            {
                foreach (var item in jadwal)
                {
                    Console.WriteLine(
                        $"{item.ScheduleId}. {item.Bus?.BusName} " +
                        $"({item.Route?.Origin} - {item.Route?.Destination})");
                }
            }

            Kembali();
        }

        // 2. DAFTAR JADWAL BUS

        static void DaftarJadwalBus()
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === DAFTAR JADWAL BUS ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .OrderBy(s => s.DepartureTime)
                .ToList();

            if (jadwal.Count == 0)
            {
                Console.WriteLine("Belum ada jadwal bus tersedia.");
            }
            else
            {
                Console.WriteLine("ID | Nama Bus | Rute | Jam Berangkat | Harga");
                Console.WriteLine("------------------------------------------------");

                foreach (var item in jadwal)
                {
                    Console.WriteLine(
                        $"{item.ScheduleId} | " +
                        $"{item.Bus?.BusName} | " +
                        $"{item.Route?.Origin} - {item.Route?.Destination} | " +
                        $"{item.DepartureTime:dd-MM-yyyy HH:mm} | " +
                        $"Rp{item.Price:N0}");
                }
            }

            Kembali();
        }

        // 3. DETAIL PERJALANAN

        static void DetailPerjalanan()
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === DETAIL PERJALANAN ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .ToList();

            if (jadwal.Count == 0)
            {
                Console.WriteLine("Belum ada jadwal perjalanan.");
                Kembali();
                return;
            }

            Console.WriteLine("Daftar Jadwal:");

            foreach (var item in jadwal)
            {
                Console.WriteLine(
                    $"{item.ScheduleId}. {item.Bus?.BusName} - " +
                    $"{item.Route?.Origin} ke {item.Route?.Destination}");
            }

            Console.Write("\nMasukkan ID Jadwal : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID jadwal tidak valid.");
                Kembali();
                return;
            }

            var detail = jadwal.FirstOrDefault(s => s.ScheduleId == id);

            if (detail == null)
            {
                Console.WriteLine("Jadwal tidak ditemukan.");
            }
            else
            {
                selectedScheduleId = detail.ScheduleId;
                selectedSeatId = null;

                Console.WriteLine("\nBus       : " + detail.Bus?.BusName);
                Console.WriteLine("Kelas     : " + detail.Bus?.BusClass);
                Console.WriteLine("Rute      : " + detail.Route?.Origin +
                                  " - " + detail.Route?.Destination);
                Console.WriteLine("Jarak     : " + detail.Route?.Distance);
                Console.WriteLine("Jam       : " + detail.DepartureTime);
                Console.WriteLine("Harga     : Rp" + detail.Price.ToString("N0"));
                Console.WriteLine("Kapasitas : " + detail.Bus?.Capacity + " kursi");
            }

            Kembali();
        }

        // 4. PEMILIHAN KURSI

        static void PemilihanKursi()
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === PEMILIHAN KURSI ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            if (selectedScheduleId == null)
            {
                Console.Write("Masukkan ID Jadwal : ");

                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID jadwal tidak valid.");
                    Kembali();
                    return;
                }

                selectedScheduleId = id;
            }

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefault(s => s.ScheduleId == selectedScheduleId);

            if (jadwal == null || jadwal.Bus == null)
            {
                Console.WriteLine("Jadwal atau bus tidak ditemukan.");
                selectedScheduleId = null;
                Kembali();
                return;
            }

            var kursi = db.Seats
                .Where(s => s.BusId == jadwal.BusId)
                .OrderBy(s => s.SeatNumber)
                .ToList();

            if (kursi.Count == 0)
            {
                for (int i = 1; i <= jadwal.Bus.Capacity; i++)
                {
                    db.Seats.Add(new Seat
                    {
                        BusId = jadwal.BusId,
                        SeatNumber = i.ToString("D2")
                    });
                }

                db.SaveChanges();

                kursi = db.Seats
                    .Where(s => s.BusId == jadwal.BusId)
                    .OrderBy(s => s.SeatNumber)
                    .ToList();
            }

            var kursiDipesan = db.Bookings
                .Where(b => b.ScheduleId == selectedScheduleId &&
                            b.Status != "Dibatalkan")
                .Select(b => b.SeatId)
                .ToList();

            Console.WriteLine("Bus: " + jadwal.Bus.BusName);
            Console.WriteLine("Rute: " + jadwal.Route?.Origin +
                              " - " + jadwal.Route?.Destination);

            Console.WriteLine("\n              BAGIAN DEPAN BUS\n");

            for (int i = 0; i < kursi.Count; i++)
            {
                bool dipesan = kursiDipesan.Contains(kursi[i].SeatId);
                string status = dipesan ? "X" : "T";

                Console.Write($"[{kursi[i].SeatNumber} {status}] ".PadRight(12));

                if ((i + 1) % 5 == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n\nKeterangan:");
            Console.WriteLine("[ T ] = Tersedia");
            Console.WriteLine("[ X ] = Sudah dipesan");

            Console.Write("\nMasukkan nomor kursi : ");
            string nomorKursi = Console.ReadLine() ?? "";

            var pilihan = kursi.FirstOrDefault(
                s => s.SeatNumber == nomorKursi.PadLeft(2, '0'));

            if (pilihan == null)
            {
                Console.WriteLine("\nNomor kursi tidak ditemukan.");
                selectedSeatId = null;
            }
            else if (kursiDipesan.Contains(pilihan.SeatId))
            {
                Console.WriteLine("\nMaaf, kursi sudah dipesan.");
                selectedSeatId = null;
            }
            else
            {
                selectedSeatId = pilihan.SeatId;

                Console.WriteLine("\nKursi " + pilihan.SeatNumber +
                                  " berhasil dipilih!");
                Console.WriteLine("Silakan lanjut ke menu Pemesanan Tiket.");
            }

            Kembali();
        }

        // 5. PEMESANAN TIKET

        static void PemesananTiket(int userId)
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === PEMESANAN TIKET ===");
            Console.WriteLine();

            if (selectedScheduleId == null || selectedSeatId == null)
            {
                Console.WriteLine("Pilih jadwal dan kursi terlebih dahulu.");
                Console.WriteLine("Gunakan menu 3 dan menu 4.");
                Kembali();
                return;
            }

            using var db = new DatabaseContext();

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefault(s => s.ScheduleId == selectedScheduleId);

            var kursi = db.Seats
                .FirstOrDefault(s => s.SeatId == selectedSeatId);

            if (jadwal == null || kursi == null)
            {
                Console.WriteLine("Data jadwal atau kursi tidak ditemukan.");
                Kembali();
                return;
            }

            bool sudahDipesan = db.Bookings.Any(b =>
                b.ScheduleId == selectedScheduleId &&
                b.SeatId == selectedSeatId &&
                b.Status != "Dibatalkan");

            if (sudahDipesan)
            {
                Console.WriteLine("Kursi sudah dipesan pengguna lain.");
                selectedSeatId = null;
                Kembali();
                return;
            }

            Console.WriteLine("Tujuan : " + jadwal.Route?.Destination);
            Console.WriteLine("Bus    : " + jadwal.Bus?.BusName);
            Console.WriteLine("Jam    : " + jadwal.DepartureTime);
            Console.WriteLine("Kursi  : " + kursi.SeatNumber);
            Console.WriteLine("Total  : Rp" + jadwal.Price.ToString("N0"));

            Console.Write("\nKonfirmasi pemesanan? (Y/N) : ");
            string konfirmasi = Console.ReadLine() ?? "";

            if (konfirmasi.ToUpper() != "Y")
            {
                Console.WriteLine("\nPemesanan dibatalkan.");
                Kembali();
                return;
            }

            string kode = "KRT" +
                Guid.NewGuid().ToString("N")[..8].ToUpper();

            db.Bookings.Add(new Booking
            {
                BookingCode = kode,
                PassengerId = userId,
                ScheduleId = jadwal.ScheduleId,
                SeatId = kursi.SeatId,
                TotalPrice = jadwal.Price,
                Status = "Menunggu Pembayaran",
                BookingDate = DateTime.Now
            });

            db.SaveChanges();

            Console.WriteLine("\nPesanan berhasil dilakukan!");
            Console.WriteLine("Kode Booking : " + kode);
            Console.WriteLine("Silakan lanjut ke menu Simulasi Pembayaran.");

            selectedSeatId = null;

            Kembali();
        }

        // 6. SIMULASI PEMBAYARAN

        static void SimulasiPembayaran(int userId)
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === SIMULASI PEMBAYARAN ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var booking = db.Bookings
                .Include(b => b.Schedule)
                .Include(b => b.Seat)
                .Where(b => b.PassengerId == userId &&
                            b.Status == "Menunggu Pembayaran")
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (booking == null)
            {
                Console.WriteLine("Tidak ada pesanan yang menunggu pembayaran.");
                Kembali();
                return;
            }

            Console.WriteLine("Kode Booking     : " + booking.BookingCode);
            Console.WriteLine("Nomor Kursi      : " + booking.Seat?.SeatNumber);
            Console.WriteLine("Total Pembayaran : Rp" +
                              booking.TotalPrice.ToString("N0"));

            Console.WriteLine("\nPilih Metode Pembayaran:");
            Console.WriteLine("[1] Transfer Bank");
            Console.WriteLine("[2] E-Wallet");
            Console.WriteLine("[3] Tunai");
            Console.Write("\nPilihan : ");

            string pilihan = Console.ReadLine() ?? "";

            string metode = pilihan switch
            {
                "1" => "Transfer Bank",
                "2" => "E-Wallet",
                "3" => "Tunai",
                _ => ""
            };

            if (metode == "")
            {
                Console.WriteLine("\nMetode pembayaran tidak valid.");
                Kembali();
                return;
            }

            db.Payments.Add(new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = metode,
                Amount = booking.TotalPrice,
                PaymentStatus = "Berhasil",
                PaymentDate = DateTime.Now
            });

            booking.Status = "Dibayar";

            db.SaveChanges();

            Console.WriteLine("\nPembayaran berhasil!");
            Console.WriteLine("Metode Pembayaran : " + metode);
            Console.WriteLine("Status            : Berhasil");

            Kembali();
        }

        // 7. E-TIKET

        static void ETiket(int userId)
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === E-TIKET ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var booking = db.Bookings
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Bus)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                .Include(b => b.Seat)
                .Where(b => b.PassengerId == userId &&
                            b.Status == "Dibayar")
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (booking == null)
            {
                Console.WriteLine("Belum ada e-tiket yang tersedia.");
                Console.WriteLine("Silakan lakukan pemesanan dan pembayaran.");
            }
            else
            {
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|             KARTABUS E-TIKET             |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("| Kode Booking : " + booking.BookingCode);
                Console.WriteLine("| Bus          : " + booking.Schedule?.Bus?.BusName);
                Console.WriteLine("| Tujuan       : " + booking.Schedule?.Route?.Destination);
                Console.WriteLine("| Jam          : " + booking.Schedule?.DepartureTime);
                Console.WriteLine("| Kursi        : " + booking.Seat?.SeatNumber);
                Console.WriteLine("| Total        : Rp" + booking.TotalPrice.ToString("N0"));
                Console.WriteLine("| Status       : LUNAS");
                Console.WriteLine("+------------------------------------------+");

                Console.WriteLine("\nSelamat menikmati perjalanan!");
            }

            Kembali();
        }

        // 8. RIWAYAT PEMESANAN

        static void RiwayatPemesanan(int userId)
        {
            Console.Clear();
            Header();

            Console.WriteLine("             === RIWAYAT PEMESANAN ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var riwayat = db.Bookings
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                .Include(b => b.Seat)
                .Where(b => b.PassengerId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            if (riwayat.Count == 0)
            {
                Console.WriteLine("Belum ada riwayat pemesanan.");
            }
            else
            {
                Console.WriteLine("Kode Booking | Tanggal | Tujuan | Kursi | Status");
                Console.WriteLine("------------------------------------------------");

                foreach (var item in riwayat)
                {
                    Console.WriteLine(
                        $"{item.BookingCode} | " +
                        $"{item.BookingDate:dd-MM-yyyy} | " +
                        $"{item.Schedule?.Route?.Destination} | " +
                        $"{item.Seat?.SeatNumber} | " +
                        $"{item.Status}");
                }
            }

            Kembali();
        }

        // =====================================================
        //                     FITUR ADMIN
        // =====================================================

        static void AdminFeature(string choice)
        {
            switch (choice)
            {
                case "1":
                    KelolaBus();
                    break;

                case "2":
                    KelolaRute();
                    break;

                case "3":
                    KelolaJadwal();
                    break;

                case "4":
                    KelolaHarga();
                    break;

                case "5":
                    KelolaKursi();
                    break;

                case "6":
                    KelolaPemesanan();
                    break;

                case "7":
                    KelolaPengguna();
                    break;
            }
        }

        // 1. KELOLA DATA BUS

        static void KelolaBus()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA DATA BUS ===");
                Console.WriteLine();

                using (var db = new DatabaseContext())
                {
                    var buses = db.Buses.OrderBy(b => b.BusId).ToList();

                    if (buses.Count == 0)
                    {
                        Console.WriteLine("Belum ada data bus.");
                    }
                    else
                    {
                        Console.WriteLine("ID | Nama Bus | Operator | Nomor Polisi | Jenis | Kapasitas");
                        Console.WriteLine("------------------------------------------------------------");

                        foreach (var bus in buses)
                        {
                            Console.WriteLine(
                                $"{bus.BusId} | {bus.BusName} | {bus.Operator} | " +
                                $"{bus.PlateNumber} | {bus.BusClass} | {bus.Capacity}");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("[1] Tambah Bus");
                Console.WriteLine("[2] Edit Data Bus");
                Console.WriteLine("[3] Hapus Data Bus");
                Console.WriteLine("[4] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        TambahBus();
                        break;

                    case "2":
                        EditBus();
                        break;

                    case "3":
                        HapusBus();
                        break;

                    case "4":
                        kembali = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void TambahBus()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === TAMBAH BUS ===");
            Console.WriteLine();

            Console.Write("Nama Bus       : ");
            string nama = Console.ReadLine() ?? "";

            Console.Write("Operator       : ");
            string operatorBus = Console.ReadLine() ?? "";

            Console.Write("Nomor Polisi   : ");
            string polisi = Console.ReadLine() ?? "";

            Console.Write("Jenis Bus      : ");
            string jenis = Console.ReadLine() ?? "";

            Console.Write("Kapasitas Kursi: ");

            if (!int.TryParse(Console.ReadLine(), out int kapasitas) ||
                kapasitas <= 0)
            {
                Message("Kapasitas harus berupa angka lebih dari 0.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nama) ||
                string.IsNullOrWhiteSpace(operatorBus) ||
                string.IsNullOrWhiteSpace(polisi) ||
                string.IsNullOrWhiteSpace(jenis))
            {
                Message("Semua data wajib diisi!");
                return;
            }

            using var db = new DatabaseContext();

            if (db.Buses.Any(b => b.PlateNumber == polisi))
            {
                Message("Nomor polisi sudah terdaftar!");
                return;
            }

            db.Buses.Add(new Bus
            {
                BusName = nama,
                Operator = operatorBus,
                PlateNumber = polisi,
                BusClass = jenis,
                Capacity = kapasitas
            });

            db.SaveChanges();

            Message("Data bus berhasil ditambahkan!");
        }

        static void EditBus()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === EDIT DATA BUS ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var buses = db.Buses.ToList();

            foreach (var bus in buses)
            {
                Console.WriteLine(
                    $"{bus.BusId}. {bus.BusName} - {bus.PlateNumber}");
            }

            Console.Write("\nMasukkan ID Bus : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID bus tidak valid.");
                return;
            }

            var data = db.Buses.FirstOrDefault(b => b.BusId == id);

            if (data == null)
            {
                Message("Data bus tidak ditemukan.");
                return;
            }

            Console.Write("Nama Bus Baru       : ");
            data.BusName = Console.ReadLine() ?? data.BusName;

            Console.Write("Operator Baru       : ");
            data.Operator = Console.ReadLine() ?? data.Operator;

            Console.Write("Nomor Polisi Baru   : ");
            string polisi = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(polisi) &&
                db.Buses.Any(b => b.PlateNumber == polisi && b.BusId != id))
            {
                Message("Nomor polisi sudah digunakan bus lain.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(polisi))
                data.PlateNumber = polisi;

            Console.Write("Jenis Bus Baru      : ");
            data.BusClass = Console.ReadLine() ?? data.BusClass;

            Console.Write("Kapasitas Baru      : ");
            string kapasitas = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(kapasitas))
            {
                if (!int.TryParse(kapasitas, out int nilai) || nilai <= 0)
                {
                    Message("Kapasitas tidak valid.");
                    return;
                }

                data.Capacity = nilai;
            }

            db.SaveChanges();

            Message("Data bus berhasil diperbarui!");
        }

        static void HapusBus()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === HAPUS DATA BUS ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            foreach (var bus in db.Buses.ToList())
            {
                Console.WriteLine($"{bus.BusId}. {bus.BusName}");
            }

            Console.Write("\nMasukkan ID Bus : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID bus tidak valid.");
                return;
            }

            var data = db.Buses.FirstOrDefault(b => b.BusId == id);

            if (data == null)
            {
                Message("Bus tidak ditemukan.");
                return;
            }

            bool punyaJadwal = db.Schedules.Any(s => s.BusId == id);

            if (punyaJadwal)
            {
                Message("Bus tidak dapat dihapus karena masih digunakan dalam jadwal.");
                return;
            }

            db.Buses.Remove(data);
            db.SaveChanges();

            Message("Data bus berhasil dihapus!");
        }

        // 2. KELOLA RUTE

        static void KelolaRute()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA RUTE ===");
                Console.WriteLine();

                using (var db = new DatabaseContext())
                {
                    var routes = db.Routes.OrderBy(r => r.RouteId).ToList();

                    if (routes.Count == 0)
                    {
                        Console.WriteLine("Belum ada data rute.");
                    }
                    else
                    {
                        Console.WriteLine("ID | Rute | Jarak");
                        Console.WriteLine("-------------------------------");

                        foreach (var rute in routes)
                        {
                            Console.WriteLine(
                                $"{rute.RouteId} | {rute.Origin} - " +
                                $"{rute.Destination} | {rute.Distance} km");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("[1] Tambah Rute");
                Console.WriteLine("[2] Edit Rute");
                Console.WriteLine("[3] Hapus Rute");
                Console.WriteLine("[4] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        TambahRute();
                        break;

                    case "2":
                        EditRute();
                        break;

                    case "3":
                        HapusRute();
                        break;

                    case "4":
                        kembali = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void TambahRute()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === TAMBAH RUTE ===");
            Console.WriteLine();

            Console.Write("Kota Asal       : ");
            string asal = Console.ReadLine() ?? "";

            Console.Write("Kota Tujuan     : ");
            string tujuan = Console.ReadLine() ?? "";

            Console.Write("Jarak (km)      : ");

            if (!double.TryParse(Console.ReadLine(), out double jarak) ||
                jarak <= 0)
            {
                Message("Jarak harus berupa angka lebih dari 0.");
                return;
            }

            if (string.IsNullOrWhiteSpace(asal) ||
                string.IsNullOrWhiteSpace(tujuan))
            {
                Message("Kota asal dan tujuan wajib diisi.");
                return;
            }

            using var db = new DatabaseContext();

            db.Routes.Add(new KARTABUS.Models.Route
            {
                Origin = asal,
                Destination = tujuan,
                Distance = jarak
            });

            db.SaveChanges();

            Message("Rute berhasil ditambahkan!");
        }

        static void EditRute()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === EDIT RUTE ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            foreach (var rute in db.Routes.ToList())
            {
                Console.WriteLine(
                    $"{rute.RouteId}. {rute.Origin} - {rute.Destination}");
            }

            Console.Write("\nMasukkan ID Rute : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID rute tidak valid.");
                return;
            }

            var data = db.Routes.FirstOrDefault(r => r.RouteId == id);

            if (data == null)
            {
                Message("Rute tidak ditemukan.");
                return;
            }

            Console.Write("Kota Asal Baru   : ");
            string asal = Console.ReadLine() ?? "";

            Console.Write("Kota Tujuan Baru : ");
            string tujuan = Console.ReadLine() ?? "";

            Console.Write("Jarak Baru (km)  : ");
            string jarak = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(asal))
                data.Origin = asal;

            if (!string.IsNullOrWhiteSpace(tujuan))
                data.Destination = tujuan;

            if (!string.IsNullOrWhiteSpace(jarak))
            {
                if (!double.TryParse(jarak, out double nilai) || nilai <= 0)
                {
                    Message("Jarak tidak valid.");
                    return;
                }

                data.Distance = nilai;
            }

            db.SaveChanges();

            Message("Data rute berhasil diperbarui!");
        }

        static void HapusRute()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === HAPUS RUTE ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            foreach (var rute in db.Routes.ToList())
            {
                Console.WriteLine(
                    $"{rute.RouteId}. {rute.Origin} - {rute.Destination}");
            }

            Console.Write("\nMasukkan ID Rute : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID rute tidak valid.");
                return;
            }

            var data = db.Routes.FirstOrDefault(r => r.RouteId == id);

            if (data == null)
            {
                Message("Rute tidak ditemukan.");
                return;
            }

            if (db.Schedules.Any(s => s.RouteId == id))
            {
                Message("Rute tidak dapat dihapus karena masih digunakan dalam jadwal.");
                return;
            }

            db.Routes.Remove(data);
            db.SaveChanges();

            Message("Rute berhasil dihapus!");
        }

        // 3. KELOLA JADWAL

        static void KelolaJadwal()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA JADWAL ===");
                Console.WriteLine();

                TampilkanJadwalAdmin();

                Console.WriteLine();
                Console.WriteLine("[1] Tambah Jadwal");
                Console.WriteLine("[2] Edit Jadwal");
                Console.WriteLine("[3] Hapus Jadwal");
                Console.WriteLine("[4] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        TambahJadwal();
                        break;

                    case "2":
                        EditJadwal();
                        break;

                    case "3":
                        HapusJadwal();
                        break;

                    case "4":
                        kembali = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void TampilkanJadwalAdmin()
        {
            using var db = new DatabaseContext();

            var jadwal = db.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .OrderBy(s => s.ScheduleId)
                .ToList();

            if (jadwal.Count == 0)
            {
                Console.WriteLine("Belum ada jadwal.");
                return;
            }

            Console.WriteLine("ID | Rute | Nama Bus | Jam Berangkat | Harga");
            Console.WriteLine("------------------------------------------------");

            foreach (var item in jadwal)
            {
                Console.WriteLine(
                    $"{item.ScheduleId} | " +
                    $"{item.Route?.Origin} - {item.Route?.Destination} | " +
                    $"{item.Bus?.BusName} | " +
                    $"{item.DepartureTime:dd-MM-yyyy HH:mm} | " +
                    $"Rp{item.Price:N0}");
            }
        }

        static void TambahJadwal()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === TAMBAH JADWAL ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            Console.WriteLine("DAFTAR BUS:");
            foreach (var bus in db.Buses.ToList())
            {
                Console.WriteLine($"{bus.BusId}. {bus.BusName}");
            }

            Console.Write("\nID Bus : ");

            if (!int.TryParse(Console.ReadLine(), out int busId) ||
                !db.Buses.Any(b => b.BusId == busId))
            {
                Message("Bus tidak ditemukan.");
                return;
            }

            Console.WriteLine("\nDAFTAR RUTE:");
            foreach (var rute in db.Routes.ToList())
            {
                Console.WriteLine(
                    $"{rute.RouteId}. {rute.Origin} - {rute.Destination}");
            }

            Console.Write("\nID Rute : ");

            if (!int.TryParse(Console.ReadLine(), out int routeId) ||
                !db.Routes.Any(r => r.RouteId == routeId))
            {
                Message("Rute tidak ditemukan.");
                return;
            }

            Console.Write("\nTanggal dan jam keberangkatan (dd-MM-yyyy HH:mm) : ");
            string waktu = Console.ReadLine() ?? "";

            if (!DateTime.TryParseExact(
                waktu,
                "dd-MM-yyyy HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime departure))
            {
                Message("Format tanggal salah. Contoh: 06-12-2026 07:00");
                return;
            }

            Console.Write("Harga Tiket : ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal harga) ||
                harga <= 0)
            {
                Message("Harga harus lebih dari 0.");
                return;
            }

            db.Schedules.Add(new Schedule
            {
                BusId = busId,
                RouteId = routeId,
                DepartureTime = departure,
                Price = harga
            });

            db.SaveChanges();

            Message("Jadwal berhasil ditambahkan!");
        }

        static void EditJadwal()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === EDIT JADWAL ===");
            Console.WriteLine();

            TampilkanJadwalAdmin();

            Console.Write("\nMasukkan ID Jadwal : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID jadwal tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var data = db.Schedules.FirstOrDefault(s => s.ScheduleId == id);

            if (data == null)
            {
                Message("Jadwal tidak ditemukan.");
                return;
            }

            Console.Write("ID Bus Baru (kosongkan jika tetap) : ");
            string busInput = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(busInput))
            {
                if (!int.TryParse(busInput, out int busId) ||
                    !db.Buses.Any(b => b.BusId == busId))
                {
                    Message("ID bus tidak ditemukan.");
                    return;
                }

                data.BusId = busId;
            }

            Console.Write("ID Rute Baru (kosongkan jika tetap) : ");
            string routeInput = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(routeInput))
            {
                if (!int.TryParse(routeInput, out int routeId) ||
                    !db.Routes.Any(r => r.RouteId == routeId))
                {
                    Message("ID rute tidak ditemukan.");
                    return;
                }

                data.RouteId = routeId;
            }

            Console.Write("Tanggal dan Jam Baru (dd-MM-yyyy HH:mm, kosongkan jika tetap) : ");
            string waktu = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(waktu))
            {
                if (!DateTime.TryParseExact(
                    waktu,
                    "dd-MM-yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime departure))
                {
                    Message("Format tanggal tidak valid.");
                    return;
                }

                data.DepartureTime = departure;
            }

            Console.Write("Harga Baru (kosongkan jika tetap) : ");
            string hargaInput = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(hargaInput))
            {
                if (!decimal.TryParse(hargaInput, out decimal harga) || harga <= 0)
                {
                    Message("Harga tidak valid.");
                    return;
                }

                data.Price = harga;
            }

            db.SaveChanges();

            Message("Jadwal berhasil diperbarui!");
        }

        static void HapusJadwal()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === HAPUS JADWAL ===");
            Console.WriteLine();

            TampilkanJadwalAdmin();

            Console.Write("\nMasukkan ID Jadwal : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID jadwal tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var data = db.Schedules.FirstOrDefault(s => s.ScheduleId == id);

            if (data == null)
            {
                Message("Jadwal tidak ditemukan.");
                return;
            }

            if (db.Bookings.Any(b => b.ScheduleId == id))
            {
                Message("Jadwal tidak dapat dihapus karena sudah memiliki pemesanan.");
                return;
            }

            db.Schedules.Remove(data);
            db.SaveChanges();

            Message("Jadwal berhasil dihapus!");
        }

        // 4. KELOLA HARGA

        static void KelolaHarga()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA HARGA ===");
                Console.WriteLine();

                TampilkanJadwalAdmin();

                Console.WriteLine();
                Console.WriteLine("[1] Ubah Harga Tiket");
                Console.WriteLine("[2] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                if (pilihan == "1")
                {
                    Console.Write("\nMasukkan ID Jadwal : ");

                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Message("ID jadwal tidak valid.");
                        continue;
                    }

                    using var db = new DatabaseContext();

                    var jadwal = db.Schedules.FirstOrDefault(s => s.ScheduleId == id);

                    if (jadwal == null)
                    {
                        Message("Jadwal tidak ditemukan.");
                        continue;
                    }

                    Console.Write("Harga Baru : ");

                    if (!decimal.TryParse(Console.ReadLine(), out decimal harga) ||
                        harga <= 0)
                    {
                        Message("Harga harus lebih dari 0.");
                        continue;
                    }

                    jadwal.Price = harga;
                    db.SaveChanges();

                    Message("Harga tiket berhasil diperbarui!");
                }
                else if (pilihan == "2")
                {
                    kembali = true;
                }
                else
                {
                    Message("Pilihan tidak valid!");
                }
            }
        }

        // 5. KELOLA KURSI

        static void KelolaKursi()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === KELOLA KURSI ===");
            Console.WriteLine();

            using var db = new DatabaseContext();

            var buses = db.Buses.ToList();

            if (buses.Count == 0)
            {
                Console.WriteLine("Belum ada data bus. Tambahkan bus terlebih dahulu.");
                Kembali();
                return;
            }

            Console.WriteLine("DAFTAR BUS:");

            foreach (var bus in buses)
            {
                Console.WriteLine(
                    $"{bus.BusId}. {bus.BusName} - Kapasitas {bus.Capacity} kursi");
            }

            Console.Write("\nMasukkan ID Bus : ");

            if (!int.TryParse(Console.ReadLine(), out int busId))
            {
                Message("ID bus tidak valid.");
                return;
            }

            var selectedBus = db.Buses.FirstOrDefault(b => b.BusId == busId);

            if (selectedBus == null)
            {
                Message("Bus tidak ditemukan.");
                return;
            }

            var kursi = db.Seats
                .Where(s => s.BusId == busId)
                .OrderBy(s => s.SeatNumber)
                .ToList();

            // Membuat kursi otomatis jika belum tersedia.
            for (int i = 1; i <= selectedBus.Capacity; i++)
            {
                string nomor = i.ToString("D2");

                if (!kursi.Any(k => k.SeatNumber == nomor))
                {
                    db.Seats.Add(new Seat
                    {
                        BusId = busId,
                        SeatNumber = nomor
                    });
                }
            }

            db.SaveChanges();

            kursi = db.Seats
                .Where(s => s.BusId == busId)
                .OrderBy(s => s.SeatNumber)
                .ToList();

            Console.WriteLine("\nDaftar Kursi Bus " + selectedBus.BusName);
            Console.WriteLine();

            foreach (var item in kursi)
            {
                Console.Write($"[{item.SeatNumber}] ");

                if (int.Parse(item.SeatNumber) % 5 == 0)
                    Console.WriteLine();
            }

            Console.WriteLine("\n");
            Console.WriteLine("[1] Lihat Status Kursi Berdasarkan Jadwal");
            Console.WriteLine("[2] Reset Kursi Bus");
            Console.WriteLine("[3] Kembali");
            Console.Write("\nPilih Opsi : ");

            string pilihan = Console.ReadLine() ?? "";

            if (pilihan == "1")
            {
                Console.Write("\nMasukkan ID Jadwal : ");

                if (!int.TryParse(Console.ReadLine(), out int scheduleId))
                {
                    Message("ID jadwal tidak valid.");
                    return;
                }

                var jadwal = db.Schedules
                    .Include(s => s.Route)
                    .FirstOrDefault(s =>
                        s.ScheduleId == scheduleId &&
                        s.BusId == busId);

                if (jadwal == null)
                {
                    Message("Jadwal untuk bus tersebut tidak ditemukan.");
                    return;
                }

                var bookedSeats = db.Bookings
                    .Where(b => b.ScheduleId == scheduleId &&
                                b.Status != "Dibatalkan")
                    .Select(b => b.SeatId)
                    .ToList();

                Console.WriteLine("\nStatus Kursi:");
                Console.WriteLine("[T] = Tersedia");
                Console.WriteLine("[X] = Sudah dipesan");
                Console.WriteLine();

                foreach (var item in kursi)
                {
                    string status = bookedSeats.Contains(item.SeatId) ? "X" : "T";
                    Console.Write($"[{item.SeatNumber} {status}] ");

                    if (int.Parse(item.SeatNumber) % 5 == 0)
                        Console.WriteLine();
                }

                Kembali();
            }
            else if (pilihan == "2")
            {
                bool adaPesanan = db.Bookings
                    .Include(b => b.Seat)
                    .Any(b => b.Seat != null && b.Seat.BusId == busId);

                if (adaPesanan)
                {
                    Message("Kursi tidak dapat direset karena sudah terkait pemesanan.");
                    return;
                }

                db.Seats.RemoveRange(kursi);
                db.SaveChanges();

                Message("Data kursi berhasil direset. Kursi akan dibuat kembali saat dibutuhkan.");
            }
            else if (pilihan == "3")
            {
                return;
            }
            else
            {
                Message("Pilihan tidak valid!");
            }
        }

        // 6. KELOLA PEMESANAN

        static void KelolaPemesanan()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA PEMESANAN ===");
                Console.WriteLine();

                TampilkanPemesananAdmin();

                Console.WriteLine();
                Console.WriteLine("[1] Lihat Detail Pemesanan");
                Console.WriteLine("[2] Ubah Status Pemesanan");
                Console.WriteLine("[3] Hapus Pemesanan");
                Console.WriteLine("[4] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        DetailPemesananAdmin();
                        break;

                    case "2":
                        UbahStatusPemesanan();
                        break;

                    case "3":
                        HapusPemesanan();
                        break;

                    case "4":
                        kembali = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void TampilkanPemesananAdmin()
        {
            using var db = new DatabaseContext();

            var data = db.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Bus)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                .Include(b => b.Seat)
                .OrderBy(b => b.BookingId)
                .ToList();

            if (data.Count == 0)
            {
                Console.WriteLine("Belum ada data pemesanan.");
                return;
            }

            Console.WriteLine("ID | Nama Penumpang | Bus | Tanggal | Status");
            Console.WriteLine("------------------------------------------------");

            foreach (var item in data)
            {
                Console.WriteLine(
                    $"{item.BookingId} | {item.Passenger?.Name} | " +
                    $"{item.Schedule?.Bus?.BusName} | " +
                    $"{item.BookingDate:dd-MM-yyyy} | {item.Status}");
            }
        }

        static void DetailPemesananAdmin()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === DETAIL PEMESANAN ===");
            Console.WriteLine();

            Console.Write("Masukkan ID Pemesanan : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID pemesanan tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var data = db.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Bus)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                .Include(b => b.Seat)
                .FirstOrDefault(b => b.BookingId == id);

            if (data == null)
            {
                Message("Pemesanan tidak ditemukan.");
                return;
            }

            Console.WriteLine("\nKode Booking : " + data.BookingCode);
            Console.WriteLine("Nama         : " + data.Passenger?.Name);
            Console.WriteLine("Bus          : " + data.Schedule?.Bus?.BusName);
            Console.WriteLine("Rute         : " +
                data.Schedule?.Route?.Origin + " - " +
                data.Schedule?.Route?.Destination);
            Console.WriteLine("Tanggal      : " + data.Schedule?.DepartureTime);
            Console.WriteLine("Kursi        : " + data.Seat?.SeatNumber);
            Console.WriteLine("Total Harga  : Rp" + data.TotalPrice.ToString("N0"));
            Console.WriteLine("Status       : " + data.Status);

            Kembali();
        }

        static void UbahStatusPemesanan()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === UBAH STATUS PEMESANAN ===");
            Console.WriteLine();

            TampilkanPemesananAdmin();

            Console.Write("\nMasukkan ID Pemesanan : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID pemesanan tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var data = db.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (data == null)
            {
                Message("Pemesanan tidak ditemukan.");
                return;
            }

            Console.WriteLine("\nStatus saat ini : " + data.Status);
            Console.WriteLine("[1] Menunggu Pembayaran");
            Console.WriteLine("[2] Dibayar");
            Console.WriteLine("[3] Dibatalkan");
            Console.Write("\nPilih Status Baru : ");

            string pilihan = Console.ReadLine() ?? "";

            data.Status = pilihan switch
            {
                "1" => "Menunggu Pembayaran",
                "2" => "Dibayar",
                "3" => "Dibatalkan",
                _ => ""
            };

            if (data.Status == "")
            {
                Message("Pilihan status tidak valid.");
                return;
            }

            db.SaveChanges();

            Message("Status pemesanan berhasil diperbarui!");
        }

        static void HapusPemesanan()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === HAPUS PEMESANAN ===");
            Console.WriteLine();

            TampilkanPemesananAdmin();

            Console.Write("\nMasukkan ID Pemesanan : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID pemesanan tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var data = db.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (data == null)
            {
                Message("Pemesanan tidak ditemukan.");
                return;
            }

            db.Bookings.Remove(data);
            db.SaveChanges();

            Message("Data pemesanan berhasil dihapus!");
        }

        // 7. KELOLA PENGGUNA

        static void KelolaPengguna()
        {
            bool kembali = false;

            while (!kembali)
            {
                Console.Clear();
                Header();

                Console.WriteLine("                 === KELOLA PENGGUNA ===");
                Console.WriteLine();

                TampilkanPenggunaAdmin();

                Console.WriteLine();
                Console.WriteLine("[1] Tambah Pengguna");
                Console.WriteLine("[2] Edit Pengguna");
                Console.WriteLine("[3] Hapus Pengguna");
                Console.WriteLine("[4] Kembali");
                Console.Write("\nPilih Opsi : ");

                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        TambahPenggunaAdmin();
                        break;

                    case "2":
                        EditPenggunaAdmin();
                        break;

                    case "3":
                        HapusPenggunaAdmin();
                        break;

                    case "4":
                        kembali = true;
                        break;

                    default:
                        Message("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static void TampilkanPenggunaAdmin()
        {
            using var db = new DatabaseContext();

            var users = db.Users.OrderBy(u => u.UserId).ToList();

            if (users.Count == 0)
            {
                Console.WriteLine("Belum ada pengguna.");
                return;
            }

            Console.WriteLine("ID | Nama | Username | Role");
            Console.WriteLine("--------------------------------------------");

            foreach (var user in users)
            {
                string role = user is Admin ? "Administrator" : "Pengguna";

                Console.WriteLine(
                    $"{user.UserId} | {user.Name} | {user.Username} | {role}");
            }
        }

        static void TambahPenggunaAdmin()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === TAMBAH PENGGUNA ===");
            Console.WriteLine();

            Console.Write("Nama Lengkap : ");
            string nama = Console.ReadLine() ?? "";

            Console.Write("Username     : ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Password     : ");
            string password = Console.ReadLine() ?? "";

            Console.Write("Nomor HP     : ");
            string phone = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nama) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone))
            {
                Message("Semua data wajib diisi!");
                return;
            }

            using var db = new DatabaseContext();

            if (db.Users.Any(u => u.Username == username))
            {
                Message("Username sudah digunakan!");
                return;
            }

            db.Passengers.Add(new Passenger
            {
                Name = nama,
                Username = username,
                Password = password,
                PhoneNumber = phone
            });

            db.SaveChanges();

            Message("Pengguna berhasil ditambahkan!");
        }

        static void EditPenggunaAdmin()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === EDIT PENGGUNA ===");
            Console.WriteLine();

            TampilkanPenggunaAdmin();

            Console.Write("\nMasukkan ID Pengguna : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID pengguna tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var user = db.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                Message("Pengguna tidak ditemukan.");
                return;
            }

            Console.Write("Nama Baru (kosongkan jika tetap) : ");
            string nama = Console.ReadLine() ?? "";

            Console.Write("Username Baru (kosongkan jika tetap) : ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Password Baru (kosongkan jika tetap) : ");
            string password = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(username) &&
                db.Users.Any(u => u.Username == username && u.UserId != id))
            {
                Message("Username sudah digunakan.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(nama))
                user.Name = nama;

            if (!string.IsNullOrWhiteSpace(username))
                user.Username = username;

            if (!string.IsNullOrWhiteSpace(password))
                user.Password = password;

            if (user is Passenger passenger)
            {
                Console.Write("Nomor HP Baru (kosongkan jika tetap) : ");
                string phone = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(phone))
                    passenger.PhoneNumber = phone;
            }
            else if (user is Admin admin)
            {
                Console.Write("Jabatan Baru (kosongkan jika tetap) : ");
                string position = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(position))
                    admin.Position = position;
            }

            db.SaveChanges();

            Message("Data pengguna berhasil diperbarui!");
        }

        static void HapusPenggunaAdmin()
        {
            Console.Clear();
            Header();

            Console.WriteLine("                 === HAPUS PENGGUNA ===");
            Console.WriteLine();

            TampilkanPenggunaAdmin();

            Console.Write("\nMasukkan ID Pengguna : ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Message("ID pengguna tidak valid.");
                return;
            }

            using var db = new DatabaseContext();

            var user = db.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                Message("Pengguna tidak ditemukan.");
                return;
            }

            if (user is Admin)
            {
                Message("Akun admin tidak dapat dihapus melalui menu ini.");
                return;
            }

            if (db.Bookings.Any(b => b.PassengerId == id))
            {
                Message("Pengguna tidak dapat dihapus karena memiliki riwayat pemesanan.");
                return;
            }

            db.Users.Remove(user);
            db.SaveChanges();

            Message("Pengguna berhasil dihapus!");
        }

        // ================= TOMBOL KEMBALI =================

        static void Kembali()
        {
            Console.WriteLine("\nTekan tombol apa saja untuk kembali...");
            Console.ReadKey();
        }

        static void Message(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("Tekan tombol apa saja untuk melanjutkan...");
            Console.ReadKey();
        }
    }
}