using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ClinicApi.DTOs;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AppointmentsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static readonly string[] AllowedStatuses = { "Scheduled", "Completed" };

    // ===================== GET LIST =====================
    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] string? status,
        [FromQuery] string? patientLastName)
    {
        var result = new List<AppointmentListDto>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT
                a.IdAppointment,
                a.AppointmentDate,
                a.Status,
                a.Reason,
                p.FirstName + ' ' + p.LastName,
                p.Email
            FROM dbo.Appointments a
            JOIN dbo.Patients p ON p.IdPatient = a.IdPatient
            WHERE (@Status IS NULL OR a.Status = @Status)
              AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)
            ORDER BY a.AppointmentDate
        ", connection);

        command.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
        command.Parameters.AddWithValue("@PatientLastName", (object?)patientLastName ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new AppointmentListDto
            {
                IdAppointment = reader.GetInt32(0),
                AppointmentDate = reader.GetDateTime(1),
                Status = reader.GetString(2),
                Reason = reader.GetString(3),
                PatientFullName = reader.GetString(4),
                PatientEmail = reader.GetString(5)
            });
        }

        return Ok(result);
    }

    // ===================== GET BY ID =====================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT 
                a.IdAppointment,
                a.AppointmentDate,
                a.Status,
                a.Reason,
                a.InternalNotes,
                a.CreatedAt,
                p.FirstName + ' ' + p.LastName,
                p.Email,
                p.PhoneNumber,
                d.FirstName + ' ' + d.LastName,
                d.LicenseNumber
            FROM dbo.Appointments a
            JOIN dbo.Patients p ON p.IdPatient = a.IdPatient
            JOIN dbo.Doctors d ON d.IdDoctor = a.IdDoctor
            WHERE a.IdAppointment = @Id
        ", connection);

        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound(new ErrorResponseDto { Message = "Appointment not found" });
        }

        var result = new AppointmentDetailsDto
        {
            IdAppointment = reader.GetInt32(0),
            AppointmentDate = reader.GetDateTime(1),
            Status = reader.GetString(2),
            Reason = reader.GetString(3),
            InternalNotes = reader.IsDBNull(4) ? null : reader.GetString(4),
            CreatedAt = reader.GetDateTime(5),
            PatientFullName = reader.GetString(6),
            PatientEmail = reader.GetString(7),
            PatientPhone = reader.GetString(8),
            DoctorFullName = reader.GetString(9),
            DoctorLicenseNumber = reader.GetString(10)
        };

        return Ok(result);
    }

    // ===================== CREATE =====================
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
    {
        // VALIDATION
        if (request.AppointmentDate < DateTime.Now)
            return BadRequest(new ErrorResponseDto { Message = "Appointment date cannot be in the past" });

        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest(new ErrorResponseDto { Message = "Reason is required" });

        if (!AllowedStatuses.Contains(request.Status))
            return BadRequest(new ErrorResponseDto { Message = "Invalid status" });

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            INSERT INTO dbo.Appointments (IdPatient, IdDoctor, AppointmentDate, Status, Reason, InternalNotes)
            VALUES (@IdPatient, @IdDoctor, @AppointmentDate, @Status, @Reason, @InternalNotes);
            SELECT SCOPE_IDENTITY();
        ", connection);

        command.Parameters.AddWithValue("@IdPatient", request.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", request.IdDoctor);
        command.Parameters.AddWithValue("@AppointmentDate", request.AppointmentDate);
        command.Parameters.AddWithValue("@Status", request.Status);
        command.Parameters.AddWithValue("@Reason", request.Reason);
        command.Parameters.AddWithValue("@InternalNotes", (object?)request.InternalNotes ?? DBNull.Value);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());

        return Created($"/api/appointments/{id}", new { Id = id });
    }

    // ===================== UPDATE =====================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] CreateAppointmentRequestDto request)
    {
        if (!AllowedStatuses.Contains(request.Status))
            return BadRequest(new ErrorResponseDto { Message = "Invalid status" });

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            UPDATE dbo.Appointments
            SET IdPatient = @IdPatient,
                IdDoctor = @IdDoctor,
                AppointmentDate = @AppointmentDate,
                Status = @Status,
                Reason = @Reason,
                InternalNotes = @InternalNotes
            WHERE IdAppointment = @Id
        ", connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@IdPatient", request.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", request.IdDoctor);
        command.Parameters.AddWithValue("@AppointmentDate", request.AppointmentDate);
        command.Parameters.AddWithValue("@Status", request.Status);
        command.Parameters.AddWithValue("@Reason", request.Reason);
        command.Parameters.AddWithValue("@InternalNotes", (object?)request.InternalNotes ?? DBNull.Value);

        var rows = await command.ExecuteNonQueryAsync();

        if (rows == 0)
            return NotFound(new ErrorResponseDto { Message = "Appointment not found" });

        return Ok(new { Message = "Updated successfully" });
    }

    // ===================== DELETE =====================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand("DELETE FROM dbo.Appointments WHERE IdAppointment = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        var rows = await command.ExecuteNonQueryAsync();

        if (rows == 0)
            return NotFound(new ErrorResponseDto { Message = "Appointment not found" });

        return NoContent();
    }
}