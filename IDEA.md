Ref Lab — Medical Laboratory Management System

Reference-Driven Reverse Engineering and Development

1. Project Overview

Develop Ref Lab, a professional Windows desktop application for managing medical diagnostic laboratories.

This project is not intended to be developed entirely from generic requirements or assumptions. An existing medical laboratory management system is available on the local computer and must be treated as the primary functional and operational reference for the new application.

The reference system is located at:

"F:\REAL_LAB_COPY"

IMPORTANT: This directory contains TWO DIFFERENT VERSIONS of the existing medical laboratory management system, not a single version.

Both versions must be independently identified, inventoried, analyzed, and compared. The agent must not assume that the two versions are identical, that one is necessarily superior, or that their files belong to the same release.

The purpose of this project is to understand the useful functionality, workflows, business rules, data structures, user-interface behavior, reporting capabilities, and other relevant characteristics of both reference versions, and then reimplement the selected functionality in Ref Lab using the specified modern technology stack and architecture.

The reference system is a source of engineering knowledge and functional requirements. Ref Lab must be a new implementation, not a direct modification of the reference software.

2. Fixed Technology Stack and Architecture

The following technical and architectural decisions are fixed and must not be changed without explicit authorization:

- Programming language: C#
- Framework: .NET 8
- Desktop user interface: Windows Presentation Foundation (WPF)
- Presentation pattern: MVVM
- Architecture: Clean Architecture
- Application request handling: CQRS with MediatR
- Database: Microsoft SQL Server
- Data access and ORM: Entity Framework Core
- Dependency injection and configuration: appropriate .NET mechanisms
- Automated testing: appropriate .NET testing frameworks

The application must maintain clear separation of concerns through the following layers:

Domain Layer

- Core business entities.
- Value objects.
- Domain rules and invariants.
- Domain interfaces where appropriate.

Application Layer

- Use cases.
- CQRS commands and queries.
- MediatR request and handler implementations.
- DTOs.
- Validation.
- Application-level business workflows.

Infrastructure Layer

- Entity Framework Core.
- SQL Server persistence.
- Database configurations.
- Repositories and data access implementations where justified.
- External integrations.

Presentation Layer

- WPF views.
- ViewModels.
- MVVM implementation.
- Navigation.
- User interaction.
- Localization and presentation resources.

Do not replace, weaken, or unnecessarily complicate these architectural decisions.

3. Reverse Engineering of the Reference System

The agent must use the available local reverse-engineering tools and capabilities to investigate the reference software.

The investigation must be evidence-driven and should use appropriate methods depending on the available artifacts, including:

- Filesystem and directory inventory.
- Identification of the two software versions.
- Static analysis of executable files and managed assemblies.
- .NET assembly metadata inspection.
- Decompilation when technically possible and appropriate.
- Inspection of configuration files.
- Database and data-model analysis.
- Analysis of report templates and printing mechanisms.
- Inspection of user-interface resources and application structure.
- Comparison of corresponding components between the two versions.
- Identification of shared functionality, differences, improvements, regressions, and version-specific behavior.

The agent must determine which analysis methods and tools are actually available before relying on them.

Do not claim that a component, feature, workflow, database structure, or business rule has been verified unless supporting evidence has been obtained.

Distinguish clearly between:

- Directly verified findings.
- Strongly supported inferences.
- Unverified assumptions.
- Information that cannot be recovered or inspected.

4. Reference System Safety and Preservation

The reference directory "F:\REAL_LAB_COPY" is strictly read-only.

The agent must not:

- Modify, rename, delete, move, or overwrite reference files.
- Install, patch, or update either reference version.
- Modify reference databases.
- Execute database write operations against reference databases.
- Apply migrations or schema changes to reference databases.
- Modify the original software configuration.
- Use the reference system as the production database for Ref Lab.
- Perform destructive or irreversible operations against reference artifacts.

Prefer static analysis over executing the reference software.

If execution is genuinely required for a specific investigation, first explain why static analysis is insufficient, identify the exact proposed action and its risks, and request explicit authorization. Do not launch the reference software or perform dynamic analysis without such authorization.

The agent must preserve the original reference artifacts and use isolated copies only when a particular analysis method requires them and authorization has been obtained.

5. Analysis of Both Versions

Both reference versions must be analyzed independently before drawing conclusions about their functionality.

The agent must establish:

- The identity and location of each version.
- The available files and components.
- The application structure and relevant dependencies.
- The database technology and available schema evidence.
- The principal workflows.
- The reporting and printing mechanisms.
- The user-interface structure.
- The capabilities and limitations of each version.

After the independent analysis, perform a structured comparison.

Do not automatically treat the newer version as the correct source of truth. A feature present in one version may be absent, changed, or defective in the other.

Where the versions disagree, document the evidence and the difference. Do not select a behavior as the intended Ref Lab requirement without a justified conclusion or explicit user decision.

6. Functional Requirements and Domain Knowledge

Use the verified findings from the reference system to establish the functional requirements of Ref Lab.

The system is intended to support the complete medical laboratory workflow, including, where supported by the reference analysis:

- Patient registration and patient records.
- Physicians, referring clinics, and referral relationships.
- Medical test catalog and test profiles.
- Laboratory orders and requested tests.
- Sample collection, identification, labeling, tracking, and processing.
- Test result entry, validation, approval, and release.
- Reference ranges, measurement units, and result interpretation.
- Laboratory reports and printing.
- Billing, payments, and financial records.
- User accounts, authentication, authorization, and audit trails.
- Laboratory settings and configuration.
- Other verified and relevant reference-system capabilities.

This list is an initial scope, not permission to invent unsupported business rules.

The agent must investigate the actual workflows and domain rules, including relevant relationships, states, validation requirements, exceptions, and user permissions.

Do not assume that all medical laboratory workflows are identical across systems.

When a requirement cannot be verified from the reference artifacts, identify the gap and request clarification when the decision materially affects the implementation.

7. Ref Lab Implementation Principles

Ref Lab must be a new, maintainable, and well-structured implementation.

The agent must:

- Reimplement the selected functionality using the fixed technology stack.
- Preserve the functional intent of verified reference workflows.
- Avoid copying obsolete implementation details when a better implementation is justified.
- Maintain data integrity and appropriate validation.
- Apply secure authentication and authorization practices.
- Protect patient data and sensitive laboratory information.
- Keep business logic independent of WPF presentation concerns.
- Avoid unnecessary abstractions, dependencies, and architectural complexity.
- Provide automated tests for implemented business rules and critical workflows.
- Maintain clear and useful technical documentation.
- Preserve existing project functionality when making changes.

Reference-system behavior must guide the implementation, but it must not override the fixed architecture or basic requirements for security, data integrity, and maintainability.

8. Database Engineering Requirements

SQL Server and Entity Framework Core are mandatory.

Database design must be based on verified domain requirements and a deliberate data-modeling process.

The agent must:

- Analyze relevant reference database evidence before designing corresponding entities.
- Document important data relationships and business constraints.
- Design the Ref Lab database independently.
- Avoid directly reusing a reference database as the application's production database.
- Avoid assuming that the reference schema should be copied unchanged.
- Inspect the existing EF Core model and migration history before proposing schema changes.
- Never apply database migrations automatically.
- Never modify an existing migration unless explicitly authorized.
- Never make database schema or data changes without explicit authorization.
- Identify database design decisions that require user approval.

Do not generate database migrations during the initial project setup or reference-analysis phase.

9. Incremental Development and User Approval

The project must be developed through small, controlled, and clearly defined phases.

The user is not an experienced programmer and relies on the coding agent to perform technical work. Therefore, the agent must provide clear explanations of significant decisions and must not assume that technical actions are automatically authorized.

The mandatory workflow is:

1. Inspect the current project state and available reference materials.
2. Analyze the relevant evidence.
3. Produce a concise report containing findings, uncertainties, and recommendations.
4. Identify the exact next proposed action.
5. Stop and wait for the user's explicit approval.
6. Implement only the approved scope.
7. Run the appropriate automated tests and verification procedures.
8. Report the exact changes, test results, limitations, and remaining work.
9. Stop and wait for authorization before proceeding to the next phase.

Do not execute multiple major phases in one uninterrupted operation.

Do not begin implementation merely because an analysis phase has been completed.

Do not interpret silence, general encouragement, or approval of a report as authorization to perform additional unapproved work.

10. Initial Project Objective

The immediate objective is NOT to implement the entire laboratory management application.

The immediate objective is to establish a reliable foundation for Ref Lab and determine how the two existing reference versions can be analyzed effectively.

The first work must focus on:

- Understanding the current development environment.
- Identifying the two reference versions inside "F:\REAL_LAB_COPY".
- Assessing the available reverse-engineering capabilities.
- Establishing a safe, read-only analysis strategy.
- Determining the most useful initial analysis phase.
- Preparing a clear, evidence-based plan for the user's approval.

No production implementation, database modification, migration generation, or reference-system modification is authorized merely by creating this project.

The agent must begin with the smallest useful and safe action, explain its purpose, and wait for the user's approval before proceeding.

11. Documentation and Reporting

Maintain concise, accurate documentation of:

- Reference version identification.
- Analysis methods and tools used.
- Verified findings and their supporting evidence.
- Differences between the two versions.
- Functional requirements derived from the reference system.
- Unresolved questions and assumptions.
- Architectural decisions.
- Approved implementation scope.
- Changes made.
- Automated test results.
- Remaining work.

Do not claim completion unless the relevant implementation and verification requirements have actually been satisfied.

Do not read the file "Docs/Audit/Plan for completing the implementation of the project.md".

Final Principle

Ref Lab must be developed as a carefully engineered, modern medical laboratory management system whose functional requirements are grounded in the investigation of the two existing reference versions, while its implementation strictly follows the specified .NET 8, WPF, MVVM, Clean Architecture, CQRS/MediatR, Entity Framework Core, and SQL Server technology stack.

Understand the reference systems first. Verify the evidence. Make no unauthorized changes. Implement incrementally. Wait for the user's approval at every major stage.
