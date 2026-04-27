📌 𝐶𝑙𝑖𝑛𝑖𝑐 𝐴𝑝𝑝𝑜𝑖𝑛𝑡𝑚𝑒𝑛𝑡 𝐴𝑃𝐼

This project is a RESTful API for managing clinic appointments.
It supports full CRUD operations, filtering, validation, and error handling.

🚀 Features
- Create, update, delete appointments
- Get all appointments (with filters)
- Get appointment by ID
- Validation (date, status, required fields)
- Conflict handling (doctor availability)
- Custom error responses
   📷 API TESTS (POSTMAN)



1️⃣ Get All Appointments

The GET /api/appointments endpoint was used to retrieve all appointments.
The response includes basic information for each appointment such as id, date, status, and patient details.
<img width="1919" height="1021" alt="Screenshot 2026-04-27 120245" src="https://github.com/user-attachments/assets/4ace7c69-3635-41db-9bc0-9643ccdcbd54" />


2️⃣ Get Appointment By ID

The GET /api/appointments/{id} endpoint was used to retrieve a specific appointment.
Detailed information such as doctor data, phone number, and internal notes was successfully returned.
<img width="1919" height="1018" alt="Screenshot 2026-04-27 120616" src="https://github.com/user-attachments/assets/667ce359-b683-47b4-8215-4312a2498dfc" />


3️⃣ Get Not Found

A request was sent with a non-existing ID.
The API correctly returned a 404 Not Found response, handling the error properly.
<img width="1919" height="1016" alt="Screenshot 2026-04-27 120635" src="https://github.com/user-attachments/assets/83542000-8f3e-46cf-aef3-c669537fe28d" />

4️⃣ Filter by Status

Filtering was performed using the ?status=Scheduled query parameter.
Only appointments with the "Scheduled" status were successfully returned.
<img width="1919" height="1020" alt="Screenshot 2026-04-27 121455" src="https://github.com/user-attachments/assets/99fc0fec-8e00-4880-864d-902d61ac3e59" />


5️⃣ POST Conflict (Doctor Busy)

An attempt was made to create a new appointment for the same doctor at the same time.
The API correctly returned a 409 Conflict, preventing overlapping appointments.
<img width="1919" height="1007" alt="Screenshot 2026-04-27 121831" src="https://github.com/user-attachments/assets/1fbd4486-7279-48f3-9111-f7b80326f4b8" />


6️⃣ PUT Without Body (Validation Error)

An update request was sent with an empty body.
The API returned a 400 Bad Request along with a validation error.
<img width="1918" height="914" alt="Screenshot 2026-04-27 122700" src="https://github.com/user-attachments/assets/df7dc0c8-dd26-43bf-9091-1a8475d775db" />


7️⃣ PUT Success

A valid PUT request was sent with proper data.
The appointment was successfully updated, returning 200 OK.
<img width="1910" height="971" alt="Screenshot 2026-04-27 122757" src="https://github.com/user-attachments/assets/a141596d-b8f7-4ff4-a92f-fc0b8afbac0d" />


8️⃣ GET After Update

After the update, the appointment was retrieved again.
The response confirmed that the changes were successfully saved.
<img width="1919" height="1019" alt="Screenshot 2026-04-27 122934" src="https://github.com/user-attachments/assets/538df3bd-a3d9-4c24-8140-16094f2b6660" />


9️⃣ DELETE Success

An existing appointment was deleted.
The API returned 204 No Content, indicating a successful deletion.
<img width="1919" height="1017" alt="Screenshot 2026-04-27 124322" src="https://github.com/user-attachments/assets/9eb3b4c8-dc4d-430d-aa89-f9dadcd99fbe" />



🔟 DELETE Not Found

An attempt was made to delete a non-existing or already deleted appointment.
The API correctly returned a 404 Not Found.
<img width="1919" height="1013" alt="Screenshot 2026-04-27 124431" src="https://github.com/user-attachments/assets/646283ac-9a59-4d31-9a40-05b0686b5a42" />


 🎉Final List

Finally, all appointments were retrieved again.
The deleted records were no longer present in the list.
<img width="1919" height="1014" alt="Screenshot 2026-04-27 124609" src="https://github.com/user-attachments/assets/d023f541-51c5-4855-8daf-1ec761c7890d" />
