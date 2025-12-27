# Lost & Found Management System

**National University of Modern Languages (NUML)**
**Department of Computer Science**

**Group Members:**
* M. Haider Ali (3347)
* M. Ashar Yousuf (3345)
* M. Abdullah (3342)
* Eman Mudassir (3358)

**Course:** Advance Programming (BSCS 4-A Morning)
**Submitted to:** Mr. Ahtesham

---

## 1. Executive Summary

**LostLink** is a robust, web-based Lost and Found Management System designed to digitize and streamline the process of recovering lost belongings within an organization, such as a university campus.

Traditional methods like physical lost-and-found boxes or notice boards are often inefficient and lack transparency. This project addresses these issues by providing a centralized, secure, and accessible platform where users can report found items, search for lost belongings, and initiate claims.

Developed using the **ASP.NET Core MVC** framework, the system ensures a responsive user experience, secure data handling, and administrative oversight. It features a complete workflow from item reporting to final reclamation, supported by a dedicated Admin Panel for moderation.

[**Watch Project Demo on YouTube**](https://youtu.be/eCPVXcoFauE?si=d8QYYIBI4B8unlg)

---

## 2. System Architecture & Interaction Design

The project is built using a modern tiered architecture, ensuring separation of concerns between data, logic, and presentation.

### 2.1. Technology Stack

* **Framework:** ASP.NET Core MVC (Model-View-Controller)
* **Language:** C#
* **Frontend:** Razor Views (.cshtml), Bootstrap 5, FontAwesome
* **Database:** Entity Framework Core (EF Core) with SQL Server/LocalDB
* **Authentication:** Cookie-based Authentication with custom session management ("MyCookieAuth")
* **API:** RESTful API endpoints for external data access

### 2.2. Use Case Analysis

The system divides functionality among three primary actors:
* **Guests:** Can browse the gallery and access static pages (About Us, Contact Us) but cannot interact with data.
* **Registered Users:** Inherit all guest privileges. Uniquely authorized to Report Items (both lost and found), Claim Items found by others, and manage their personal Profile Dashboard.
* **Administrators:** Possess elevated privileges to moderate content, including cascading deletion of users and adjudication of claims (Approve/Reject).
---

## 3. Database Design & Data Models

The system uses **Entity Framework Core** to manage the database via the `ApplicationDbContext` class. The schema is normalized to ensure data integrity and efficient querying.

### 3.1. Entity Relationship Diagram (ERD)

* **Users Table:** The central entity. Has a One-to-Many relationship with `FoundItems` (a user can report multiple items) and `Claims` (a user can claim multiple items).
* **FoundItems Table:** Linked to the User who reported it. Acts as the parent entity for Claims.
* **Claims Table:** A crucial associative entity connecting a User (the claimant) to a FoundItem. Tracks the lifecycle of a specific transaction request via the `Status` field.
* **ContactUs:** A standalone table for storing user inquiries, decoupled from the authentication system.

---

## 4. Detailed Modules & Functionalities

### 4.1. User Identity Module
* **Registration:** The `SignupController` handles new user registration, supporting profile picture uploads to the server's `wwwroot` directory.
* **Authentication:** The `LoginController` creates a secure identity. A custom Session Token mechanism prevents stale session persistence after server restarts by checking `ActiveServerSessions` in memory.

### 4.2. Item Reporting & Gallery
* **Reporting:** Authorized users access `Found.cshtml` to submit item details. Server-side validation ensures data integrity.
* **Gallery:** `HomeController.Index` displays the 6 most recent items, while `Gallery.cshtml` provides a full archive.

### 4.3. Claiming System & Workflow
The core business logic resides in the claiming process.
1.  **Submission:** User transitions claim state from NonExistent to Pending by submitting the form.
2.  **Waiting Period:** The claim remains Pending until an Administrator logs in.
3.  **Review:** Admin enters the "Decision Window," reviewing proof of ownership against the item description.
4.  **Resolution:** Admin transitions state to Approved (or Rejected), closing the workflow.

### 4.4. Admin Module
* **User Management:** Admins can view and delete users. Deletion logic cascades to remove associated claims, preventing orphaned records.
* **Claim Adjudication:** The "All Claims" view allows Admins to inspect pending claims and toggle their status.

---

## 5. Security Features

* **Authorization:** `[Authorize]` attributes protect sensitive routes (Profile, Reporting).
* **CSRF Protection:** `@Html.AntiForgeryToken()` is implemented on all forms.
* **Data Validation:** Strict server-side validation on models prevents invalid data injection.
* **Session Security:** Memory-based session tracking ensures that sessions are invalidated correctly upon server reset.

---

## 6. Conclusion

Project demonstrates a practical application of advanced C# and ASP.NET Core concepts. By integrating complex relationships (Users-Items-Claims), secure authentication, and a clear administrative workflow, it provides a scalable solution for campus inventory management.
