var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

// ---------- Datos ----------
var canchas = new List<Cancha> {
    new(1, "Cancha Principal", "Fútbol 7", "Grass sintético", 14, 60,
        "Cancha techada con iluminación LED y vestuarios.",
        "https://images.unsplash.com/photo-1459865264687-595d652de67e?w=800"),
    new(2, "Cancha Norte", "Fútbol 5", "Grass sintético", 10, 45,
        "Ideal para partidos rápidos entre amigos.",
        "https://images.unsplash.com/photo-1529900748604-07564a03e7a6?w=800"),
    new(3, "Cancha Sur", "Fútbol 11", "Grass natural", 22, 120,
        "Cancha grande para campeonatos y entrenamientos.",
        "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800"),
};

var servicios = new List<Servicio> {
    new(1, "Alquiler de cancha", "Principal", "Reserva por hora, con balón incluido.", 60, "⚽"),
    new(2, "Campeonatos", "Eventos", "Organización de torneos con árbitros.", 350, "🏆"),
    new(3, "Entrenamientos", "Deporte", "Clases con entrenador certificado.", 40, "🎯"),
    new(4, "Vestuarios", "Comodidad", "Vestuarios con duchas y casilleros.", 0, "🚿"),
    new(5, "Iluminación nocturna", "Comodidad", "Reflectores LED para partidos de noche.", 20, "💡"),
    new(6, "Estacionamiento", "Comodidad", "Espacio vigilado para autos y motos.", 5, "🅿️"),
};

var promociones = new List<Promocion> {
    new(1, "2x1 los martes", "Paga una hora y juega dos.", 50, "2026-12-31"),
    new(2, "Pack de equipo", "Reserva 4 horas al mes y obtén descuento.", 15, "2026-11-30"),
    new(3, "Hora temprana", "Descuento antes de las 6 pm.", 20, "2026-12-31"),
};

// Horarios de los próximos 7 días (hora de Perú)
var hoy = DateTime.UtcNow.AddHours(-5).Date;
var horarios = new List<Horario>();
int idH = 1;
for (int d = 0; d < 7; d++)
    foreach (var c in canchas)
        for (int h = 16; h <= 22; h++)
            horarios.Add(new Horario(idH++, c.Id,
                hoy.AddDays(d).ToString("yyyy-MM-dd"),
                $"{h:00}:00", $"{h + 1:00}:00", 60,
                h < 18 ? c.PrecioBase : c.PrecioBase + 20,
                (d + h + c.Id) % 3 != 0));

// ---------- Endpoints ----------
app.MapGet("/", () => "API Cancha funcionando");
app.MapGet("/api/canchas", () => canchas);
app.MapGet("/api/canchas/{id:int}", (int id) =>
    canchas.FirstOrDefault(c => c.Id == id) is { } c ? Results.Ok(c) : Results.NotFound());
app.MapGet("/api/servicios", () => servicios);
app.MapGet("/api/promociones", () => promociones);
app.MapGet("/api/horarios", (string? fecha, int? canchaId, bool? disponible) =>
    horarios.Where(h =>
        (fecha is null || h.Fecha == fecha) &&
        (canchaId is null || h.CanchaId == canchaId) &&
        (disponible is null || h.Disponible == disponible)));

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

record Cancha(int Id, string Nombre, string Tipo, string Superficie, int Capacidad,
              int PrecioBase, string Descripcion, string Imagen);
record Servicio(int Id, string Nombre, string Categoria, string Descripcion, int Precio, string Icono);
record Promocion(int Id, string Titulo, string Descripcion, int DescuentoPct, string ValidoHasta);
record Horario(int Id, int CanchaId, string Fecha, string HoraInicio, string HoraFin,
               int DuracionMin, int Precio, bool Disponible);
