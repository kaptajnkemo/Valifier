### **EPIC-1: Global Admin Platform Bootstrap**
- **Category:** USER-FACING
- *Objective: Establish the first authenticated platform workflow by allowing a Global Admin to sign in, create tenant records, inspect tenant bootstrap data, and provision the first tenant superuser access path into the system.*

- [x] **SIP-1.1: Global Admin can sign in and reach the admin dashboard**
  - **Summary:** The system provides a public home page at `/`, a shared sign-in page at `/sign-in`, and redirects valid Global Admin sessions to `/admin/dashboard`.
  - **Objective (WHY):** The platform needs an authenticated entry point for platform-level operations.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** None.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/` displays a `Sign in` action that opens `/sign-in`.
    2. The route `/sign-in` displays an email input, a password input, and a submit action.
    3. Submission of valid Global Admin credentials on `/sign-in` redirects to `/admin/dashboard`.
    4. A request to `/admin/dashboard` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser navigation to `/` shows a `Sign in` action that opens `/sign-in`**
    2. **AT2 - Browser navigation to `/sign-in` shows an email input, a password input, and a submit action**
    3. **AT3 - Browser submission of valid Global Admin credentials on `/sign-in` redirects to `/admin/dashboard`**
    4. **AT4 - Browser navigation to `/admin/dashboard` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant creation. Tenant detail view. Tenant-superuser sign-in.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-1.2: Global Admin dashboard shows tenant counts and tenant list rows**
  - **Summary:** The Global Admin dashboard at `/admin/dashboard` displays tenant count, tenant list rows, and navigation actions to tenant creation and tenant detail routes.
  - **Objective (WHY):** The Global Admin needs one landing page that exposes current tenant bootstrap state.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.1.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/admin/dashboard` displays the total number of tenant records.
    2. The route `/admin/dashboard` displays one tenant list row per tenant record.
    3. The route `/admin/dashboard` displays the text `No tenants found` when zero tenant records exist.
    4. The route `/admin/dashboard` displays a `Create tenant` action that opens `/admin/tenants/new`.
    5. Each tenant list row on `/admin/dashboard` displays a `View tenant` action that opens `/admin/tenants/{tenantId}`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/admin/dashboard` with three tenant records shows the total tenant count value `3`**
    2. **AT2 - Browser view of `/admin/dashboard` with three tenant records shows three tenant list rows**
    3. **AT3 - Browser view of `/admin/dashboard` with zero tenant records shows the text `No tenants found`**
    4. **AT4 - Browser activation of the `Create tenant` action on `/admin/dashboard` opens `/admin/tenants/new`**
    5. **AT5 - Browser activation of a `View tenant` action on `/admin/dashboard` opens `/admin/tenants/{tenantId}`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Monitoring beyond tenant count and tenant list rows. Tenant-superuser sign-in.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-1.3: Global Admin can create a tenant and initial tenant superuser**
  - **Summary:** The route `/admin/tenants/new` allows a Global Admin to submit tenant and initial tenant superuser values and return to `/admin/dashboard`.
  - **Objective (WHY):** Tenant-scoped access must be created by the platform instead of manual setup outside the system.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.1, SIP-1.2.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/admin/tenants/new` displays inputs for tenant name, initial tenant superuser display name, initial tenant superuser email address, initial tenant superuser password, and a submit action.
    2. Submission of valid values on `/admin/tenants/new` redirects to `/admin/dashboard`.
    3. Browser view of `/admin/dashboard` after a valid submission shows the total tenant count increased by `1`.
    4. Browser view of `/admin/dashboard` after a valid submission shows one tenant list row with the submitted tenant name.
    5. Submission of a form with a missing required value on `/admin/tenants/new` remains on `/admin/tenants/new`.
    6. Submission of a form with a missing required value on `/admin/tenants/new` does not increase the total tenant count displayed on `/admin/dashboard`.
    7. A request to `/admin/tenants/new` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser navigation to `/admin/tenants/new` shows inputs for tenant name, initial tenant superuser display name, initial tenant superuser email address, initial tenant superuser password, and a submit action**
    2. **AT2 - Browser submission of valid values on `/admin/tenants/new` redirects to `/admin/dashboard`**
    3. **AT3 - Browser view of `/admin/dashboard` after a valid submission shows the total tenant count increased by `1`**
    4. **AT4 - Browser view of `/admin/dashboard` after a valid submission shows one tenant list row with the submitted tenant name**
    5. **AT5 - Browser submission of a form with a missing required value on `/admin/tenants/new` remains on `/admin/tenants/new`**
    6. **AT6 - Browser view of `/admin/dashboard` after submission of a form with a missing required value on `/admin/tenants/new` shows the total tenant count unchanged**
    7. **AT7 - Browser navigation to `/admin/tenants/new` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant editing. Tenant deletion. Invitation email delivery. Password reset.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-1.4: Initial tenant superuser can sign in and reach the tenant dashboard**
  - **Summary:** The system uses the shared sign-in page at `/sign-in` and redirects valid tenant-superuser sessions to `/tenant/dashboard`.
  - **Objective (WHY):** Tenant access must be established by the provisioning workflow before tenant-scoped operations can begin.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.3.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/sign-in` displays an email input, a password input, and a submit action.
    2. Submission of valid initial tenant superuser credentials on `/sign-in` redirects to `/tenant/dashboard`.
    3. The route `/tenant/dashboard` displays the tenant name.
    4. The route `/tenant/dashboard` displays the signed-in user email address.
    5. A request to `/tenant/dashboard` without an authenticated tenant-superuser session redirects to `/sign-in`.
    6. A request to `/admin/dashboard` with an authenticated tenant-superuser session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser navigation to `/sign-in` shows an email input, a password input, and a submit action**
    2. **AT2 - Browser submission of valid initial tenant superuser credentials on `/sign-in` redirects to `/tenant/dashboard`**
    3. **AT3 - Browser view of `/tenant/dashboard` shows the tenant name**
    4. **AT4 - Browser view of `/tenant/dashboard` shows the signed-in user email address**
    5. **AT5 - Browser navigation to `/tenant/dashboard` without an authenticated tenant-superuser session redirects to `/sign-in`**
    6. **AT6 - Browser navigation to `/admin/dashboard` with an authenticated tenant-superuser session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant user administration. Hiring manager provisioning. Tenant dashboard metrics beyond tenant name and signed-in user email address.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-1.5: Global Admin can inspect tenant detail and first-sign-in state**
  - **Summary:** The route `/admin/tenants/{tenantId}` displays tenant identity values and first-sign-in state for the initial tenant superuser.
  - **Objective (WHY):** The Global Admin needs a tenant drill-down page that shows whether the provisioned tenant has been accessed by its first tenant superuser.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.2, SIP-1.3, SIP-1.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/admin/tenants/{tenantId}` is reachable by activating a `View tenant` action on `/admin/dashboard`.
    2. The route `/admin/tenants/{tenantId}` displays the tenant name.
    3. The route `/admin/tenants/{tenantId}` displays the initial tenant superuser display name.
    4. The route `/admin/tenants/{tenantId}` displays the initial tenant superuser email address.
    5. The route `/admin/tenants/{tenantId}` displays the text `First sign-in completed: Yes` when the initial tenant superuser has completed a successful sign-in.
    6. The route `/admin/tenants/{tenantId}` displays the text `First sign-in completed: No` when the initial tenant superuser has not completed a successful sign-in.
    7. A request to `/admin/tenants/{tenantId}` for a non-existent tenant record displays the text `Tenant not found`.
    8. A request to `/admin/tenants/{tenantId}` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of a `View tenant` action on `/admin/dashboard` opens `/admin/tenants/{tenantId}`**
    2. **AT2 - Browser view of `/admin/tenants/{tenantId}` shows the tenant name**
    3. **AT3 - Browser view of `/admin/tenants/{tenantId}` shows the initial tenant superuser display name**
    4. **AT4 - Browser view of `/admin/tenants/{tenantId}` shows the initial tenant superuser email address**
    5. **AT5 - Browser view of `/admin/tenants/{tenantId}` for a tenant whose initial tenant superuser has completed a successful sign-in shows the text `First sign-in completed: Yes`**
    6. **AT6 - Browser view of `/admin/tenants/{tenantId}` for a tenant whose initial tenant superuser has not completed a successful sign-in shows the text `First sign-in completed: No`**
    7. **AT7 - Browser navigation to `/admin/tenants/{tenantId}` for a non-existent tenant record shows the text `Tenant not found`**
    8. **AT8 - Browser navigation to `/admin/tenants/{tenantId}` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant editing. Tenant suspension. Cross-tenant impersonation. Platform telemetry dashboards.
  - **Status:** Completed 2026-03-24.

### **EPIC-2: Compliance, Audit, and Data Governance Foundation**
- **Category:** SYSTEM-ONLY
- *Objective: Establish verifiable recording of committed data changes, governed data metadata, and privacy-request state so platform operations can be traced and governed before broader recruitment workflows expand.*

- [x] **SIP-2.1: Committed data changes create transaction audit records**
  - **Summary:** The system creates one transaction audit record for each committed create, update, and delete operation on governed records.
  - **Objective (WHY):** Traceability of data changes must exist even when an application service does not emit a business audit event.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.3, SIP-1.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. A committed create operation on a governed record creates one transaction audit record.
    2. A committed update operation on a governed record creates one transaction audit record.
    3. A committed delete operation on a governed record creates one transaction audit record.
    4. A write operation that does not commit creates no transaction audit record.
    5. Each transaction audit record contains transaction identifier, UTC commit timestamp, operation type, record type, record identifier, actor identifier or `Anonymous`, and tenant identifier or `Platform`.
    6. An attempt to modify an existing transaction audit record is rejected.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Audit-store inspection after one committed create operation shows one transaction audit record**
    2. **AT2 - Audit-store inspection after one committed update operation shows one transaction audit record**
    3. **AT3 - Audit-store inspection after one committed delete operation shows one transaction audit record**
    4. **AT4 - Audit-store inspection after one write operation that does not commit shows no new transaction audit record**
    5. **AT5 - Audit-store inspection of one transaction audit record shows transaction identifier, UTC commit timestamp, operation type, record type, record identifier, actor identifier or `Anonymous`, and tenant identifier or `Platform`**
    6. **AT6 - Submission of an update request for an existing transaction audit record returns a rejection result**
  - **Explicit Non-Goals (OUT OF SCOPE):** Dashboard metrics. Trendlines. Business-event taxonomy. External log aggregation.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-2.2: Governed records expose compliance metadata**
  - **Summary:** The system exposes compliance metadata for governed user and tenant-scoped records through an application contract.
  - **Objective (WHY):** Retention, lawful handling, and request processing require governed records to carry observable compliance metadata.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.3, SIP-1.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. A governed record exposes one data category code.
    2. A governed record exposes one retention rule code.
    3. A governed record exposes one collection timestamp.
    4. A governed record exposes one subject-scope value.
    5. A request for compliance metadata for a non-existent governed record returns `Record not found`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - API request for compliance metadata for an existing governed record returns one data category code**
    2. **AT2 - API request for compliance metadata for an existing governed record returns one retention rule code**
    3. **AT3 - API request for compliance metadata for an existing governed record returns one collection timestamp**
    4. **AT4 - API request for compliance metadata for an existing governed record returns one subject-scope value**
    5. **AT5 - API request for compliance metadata for a non-existent governed record returns `Record not found`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Retention execution. Data export payload delivery. Data erasure execution.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-2.3: Data-subject requests can be recorded and state-tracked**
  - **Summary:** The system records privacy requests for a subject and exposes request status through an application contract.
  - **Objective (WHY):** Privacy operations need a system record of incoming requests before fulfillment workflows are added.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-2.2.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The system creates one request record for a valid access request with status `Received`.
    2. The system creates one request record for a valid erasure request with status `Received`.
    3. The system rejects a privacy request when subject identifier is missing.
    4. The system exposes the status values `Received`, `InReview`, `Completed`, and `Rejected`.
    5. Each request record contains request identifier, subject identifier, request type, submitted timestamp, and current status.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - API submission of a valid access request creates one request record with status `Received`**
    2. **AT2 - API submission of a valid erasure request creates one request record with status `Received`**
    3. **AT3 - API submission of a privacy request with a missing subject identifier returns a rejection result**
    4. **AT4 - API request for privacy request details returns one of the status values `Received`, `InReview`, `Completed`, or `Rejected`**
    5. **AT5 - API request for privacy request details returns request identifier, subject identifier, request type, submitted timestamp, and current status**
  - **Explicit Non-Goals (OUT OF SCOPE):** Candidate-facing request submission UI. Data export generation. Data deletion execution.
  - **Status:** Completed 2026-03-24.

- [x] **SIP-2.4: Compliance contracts are schema-verified**
  - **Summary:** The system validates transaction audit, compliance metadata, and privacy request contracts through schema-focused tests.
  - **Objective (WHY):** A SYSTEM-ONLY compliance epic requires explicit verification of the contracts that later workflows and oversight views will consume.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-2.1, SIP-2.2, SIP-2.3.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The transaction audit contract requires transaction identifier, UTC commit timestamp, operation type, record type, and record identifier fields.
    2. The compliance metadata contract requires record identifier, data category code, retention rule code, collection timestamp, and subject-scope fields.
    3. The privacy request contract requires request identifier, subject identifier, request type, submitted timestamp, and status fields.
    4. A contract payload missing a required field is rejected.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Contract test for the transaction audit schema fails when one required field is missing**
    2. **AT2 - Contract test for the compliance metadata schema fails when one required field is missing**
    3. **AT3 - Contract test for the privacy request schema fails when one required field is missing**
    4. **AT4 - Contract test for each schema passes when all required fields are present**
  - **Explicit Non-Goals (OUT OF SCOPE):** Dashboard rendering. Alerting rules. Reporting calculations.
  - **Status:** Completed 2026-03-24.

### **EPIC-3: Tenant Operations Bootstrap**
- **Category:** USER-FACING
- *Objective: Establish the first tenant-side operating workflow by allowing tenant roles to reach the tenant workspace, allowing tenant superusers to create Hiring Manager users and tenant-scoped sources of truth, and allowing tenant recruitment projects to be created with one Hiring Manager owner and tenant-scoped context references.*

- [x] **SIP-3.1: Tenant dashboard exposes role-scoped navigation and counts**
  - **Summary:** The route `/tenant/dashboard` displays tenant operation counts and role-scoped navigation actions.
  - **Objective (WHY):** Tenant roles need one reachable workspace entry point for tenant operations without guessing URLs.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. Browser view of `/tenant/dashboard` with a tenant-superuser session displays the total number of tenant users.
    2. Browser view of `/tenant/dashboard` with a tenant-superuser session displays the total number of tenant sources of truth.
    3. Browser view of `/tenant/dashboard` with a tenant-superuser session displays the total number of tenant recruitment projects.
    4. Browser view of `/tenant/dashboard` with a tenant-superuser session displays a `Users` action that opens `/tenant/users`.
    5. Browser view of `/tenant/dashboard` with a tenant-superuser session displays a `Sources of truth` action that opens `/tenant/sots`.
    6. Browser view of `/tenant/dashboard` with a tenant-superuser session displays a `Projects` action that opens `/tenant/projects`.
    7. Browser view of `/tenant/dashboard` with a Hiring Manager session displays a `Projects` action that opens `/tenant/projects`.
    8. Browser view of `/tenant/dashboard` with a Hiring Manager session does not display a `Users` action.
    9. Browser view of `/tenant/dashboard` with a Hiring Manager session does not display a `Sources of truth` action.
    10. A request to `/tenant/dashboard` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/tenant/dashboard` with two tenant users shows the total tenant user count value `2`**
    2. **AT2 - Browser view of `/tenant/dashboard` with three tenant sources of truth shows the total source-of-truth count value `3`**
    3. **AT3 - Browser view of `/tenant/dashboard` with four tenant recruitment projects shows the total project count value `4`**
    4. **AT4 - Browser activation of the `Users` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/users`**
    5. **AT5 - Browser activation of the `Sources of truth` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/sots`**
    6. **AT6 - Browser activation of the `Projects` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/projects`**
    7. **AT7 - Browser activation of the `Projects` action on `/tenant/dashboard` with a Hiring Manager session opens `/tenant/projects`**
    8. **AT8 - Browser view of `/tenant/dashboard` with a Hiring Manager session does not show a `Users` action**
    9. **AT9 - Browser view of `/tenant/dashboard` with a Hiring Manager session does not show a `Sources of truth` action**
    10. **AT10 - Browser navigation to `/tenant/dashboard` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** User creation. Source-of-truth creation. Project creation. Global Admin tenant oversight.
  - **Status:** Completed 2026-03-25.

- [x] **SIP-3.2: Tenant superuser can create Hiring Manager users**
  - **Summary:** The routes `/tenant/users` and `/tenant/users/new` allow a tenant superuser to create Hiring Manager users and make those users able to sign in to `/tenant/dashboard`.
  - **Objective (WHY):** Tenant project ownership requires tenant-owned user administration before recruitment work begins.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/tenant/users` is reachable by activating the `Users` action on `/tenant/dashboard` with a tenant-superuser session.
    2. Browser view of `/tenant/users` with a tenant-superuser session displays one user list row per tenant user.
    3. Browser view of `/tenant/users` with a tenant-superuser session does not display users from another tenant.
    4. Browser view of `/tenant/users` with a tenant-superuser session displays a `Create Hiring Manager` action that opens `/tenant/users/new`.
    5. The route `/tenant/users/new` displays a display name input, an email address input, a password input, and a submit action.
    6. Submission of valid values on `/tenant/users/new` redirects to `/tenant/users`.
    7. Browser view of `/tenant/users` after a valid submission shows one user list row with the submitted email address and the role `Hiring Manager`.
    8. Submission of a form with a missing required value on `/tenant/users/new` remains on `/tenant/users/new`.
    9. Submission of valid created Hiring Manager credentials on `/sign-in` redirects to `/tenant/dashboard`.
    10. A request to `/tenant/users` with an authenticated Hiring Manager session redirects to `/sign-in`.
    11. A request to `/tenant/users/new` with an authenticated Hiring Manager session redirects to `/sign-in`.
    12. A request to `/tenant/users` without an authenticated tenant-superuser session redirects to `/sign-in`.
    13. A request to `/tenant/users/new` without an authenticated tenant-superuser session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the `Users` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/users`**
    2. **AT2 - Browser view of `/tenant/users` with two tenant users and one different-tenant user shows two user list rows**
    3. **AT3 - Browser activation of the `Create Hiring Manager` action on `/tenant/users` opens `/tenant/users/new`**
    4. **AT4 - Browser view of `/tenant/users/new` shows a display name input, an email address input, a password input, and a submit action**
    5. **AT5 - Browser submission of valid values on `/tenant/users/new` redirects to `/tenant/users`**
    6. **AT6 - Browser view of `/tenant/users` after a valid submission shows one user list row with the submitted email address and the role `Hiring Manager`**
    7. **AT7 - Browser submission of a form with a missing required value on `/tenant/users/new` remains on `/tenant/users/new`**
    8. **AT8 - Browser submission of valid created Hiring Manager credentials on `/sign-in` redirects to `/tenant/dashboard`**
    9. **AT9 - Browser navigation to `/tenant/users` with an authenticated Hiring Manager session redirects to `/sign-in`**
    10. **AT10 - Browser navigation to `/tenant/users/new` with an authenticated Hiring Manager session redirects to `/sign-in`**
    11. **AT11 - Browser navigation to `/tenant/users` without an authenticated tenant-superuser session redirects to `/sign-in`**
    12. **AT12 - Browser navigation to `/tenant/users/new` without an authenticated tenant-superuser session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant-superuser creation. User editing. Password reset. User suspension.
  - **Status:** Completed 2026-03-25.

- [x] **SIP-3.3: Tenant superuser can create tenant-scoped sources of truth**
  - **Summary:** The routes `/tenant/sots`, `/tenant/sots/new`, and `/tenant/sots/{sotId}` allow a tenant superuser to create tenant-scoped sources of truth with topic, name, schema version, and generic entries.
  - **Objective (WHY):** Downstream tenant workflows need reusable tenant-scoped context records that are not hardcoded to one business topic.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/tenant/sots` is reachable by activating the `Sources of truth` action on `/tenant/dashboard` with a tenant-superuser session.
    2. Browser view of `/tenant/sots` with a tenant-superuser session displays one source-of-truth list row per tenant source of truth.
    3. Browser view of `/tenant/sots` with a tenant-superuser session does not display sources of truth from another tenant.
    4. Browser view of `/tenant/sots` with a tenant-superuser session displays a `Create source of truth` action that opens `/tenant/sots/new`.
    5. The route `/tenant/sots/new` displays a topic input, a name input, a schema version input, an entry key input, an entry label input, an entry value type selection, an entry value input, an `Add entry` action, and a `Save source of truth` action.
    6. Activation of the `Add entry` action on `/tenant/sots/new` with valid entry values displays one entry list row with the submitted key, label, value type, and value.
    7. Submission of valid values on `/tenant/sots/new` redirects to `/tenant/sots`.
    8. Browser view of `/tenant/sots` after a valid submission shows one source-of-truth list row with the submitted topic and the submitted name.
    9. The route `/tenant/sots/{sotId}` is reachable by activating a `View source of truth` action on `/tenant/sots`.
    10. Browser view of `/tenant/sots/{sotId}` shows the saved topic, the saved name, the saved schema version, and the saved entry list rows.
    11. Submission of a form with a missing required value on `/tenant/sots/new` remains on `/tenant/sots/new`.
    12. A request to `/tenant/sots` with an authenticated Hiring Manager session redirects to `/sign-in`.
    13. A request to `/tenant/sots/new` with an authenticated Hiring Manager session redirects to `/sign-in`.
    14. A request to `/tenant/sots/new` without an authenticated tenant-superuser session redirects to `/sign-in`.
    15. A request to `/tenant/sots/{sotId}` for a source of truth from another tenant shows the text `Source of truth not found`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the `Sources of truth` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/sots`**
    2. **AT2 - Browser view of `/tenant/sots` with two tenant sources of truth and one different-tenant source of truth shows two source-of-truth list rows**
    3. **AT3 - Browser activation of the `Create source of truth` action on `/tenant/sots` opens `/tenant/sots/new`**
    4. **AT4 - Browser view of `/tenant/sots/new` shows a topic input, a name input, a schema version input, an entry key input, an entry label input, an entry value type selection, an entry value input, an `Add entry` action, and a `Save source of truth` action**
    5. **AT5 - Browser activation of the `Add entry` action on `/tenant/sots/new` with valid entry values shows one entry list row with the submitted key, label, value type, and value**
    6. **AT6 - Browser submission of valid values on `/tenant/sots/new` redirects to `/tenant/sots`**
    7. **AT7 - Browser view of `/tenant/sots` after a valid submission shows one source-of-truth list row with the submitted topic and the submitted name**
    8. **AT8 - Browser activation of a `View source of truth` action on `/tenant/sots` opens `/tenant/sots/{sotId}`**
    9. **AT9 - Browser view of `/tenant/sots/{sotId}` shows the saved topic, the saved name, the saved schema version, and the saved entry list rows**
    10. **AT10 - Browser submission of a form with a missing required value on `/tenant/sots/new` remains on `/tenant/sots/new`**
    11. **AT11 - Browser navigation to `/tenant/sots` with an authenticated Hiring Manager session redirects to `/sign-in`**
    12. **AT12 - Browser navigation to `/tenant/sots/new` with an authenticated Hiring Manager session redirects to `/sign-in`**
    13. **AT13 - Browser navigation to `/tenant/sots/new` without an authenticated tenant-superuser session redirects to `/sign-in`**
    14. **AT14 - Browser navigation to `/tenant/sots/{sotId}` for a source of truth from another tenant shows the text `Source of truth not found`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Source-of-truth editing. Topic-specific field templates. Full workflow-specific source-of-truth consumption rules. Multi-source-of-truth composition. Topic-specific source-of-truth processing logic.
  - **Status:** Completed 2026-03-25.

- [x] **SIP-3.4: Tenant roles can create recruitment projects with one Hiring Manager owner and one tenant-scoped source-of-truth reference**
  - **Summary:** The routes `/tenant/projects`, `/tenant/projects/new`, and `/tenant/projects/{projectId}` allow tenant roles to create recruitment projects with one Hiring Manager owner and one tenant-scoped source-of-truth reference.
  - **Objective (WHY):** Recruitment work must begin inside a tenant project container with one valid owner and one reusable tenant-scoped context reference.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1, SIP-3.2, SIP-3.3.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/tenant/projects` is reachable by activating the `Projects` action on `/tenant/dashboard` with a tenant-superuser session.
    2. The route `/tenant/projects` is reachable by activating the `Projects` action on `/tenant/dashboard` with a Hiring Manager session.
    3. Browser view of `/tenant/projects` with a tenant-superuser session displays one project list row per tenant recruitment project.
    4. Browser view of `/tenant/projects` with a Hiring Manager session displays only project list rows owned by the signed-in Hiring Manager.
    5. Browser view of `/tenant/projects` with a tenant-superuser session displays a `Create project` action that opens `/tenant/projects/new`.
    6. Browser view of `/tenant/projects` with a Hiring Manager session displays a `Create project` action that opens `/tenant/projects/new`.
    7. The route `/tenant/projects/new` with a tenant-superuser session displays a job title input, a department input, a Hiring Manager owner selection, a source-of-truth selection, and a submit action.
    8. The Hiring Manager owner selection on `/tenant/projects/new` with a tenant-superuser session contains tenant Hiring Manager users only.
    9. The route `/tenant/projects/new` with a Hiring Manager session displays a job title input, a department input, a source-of-truth selection, and a submit action.
    10. The route `/tenant/projects/new` with a Hiring Manager session does not display a Hiring Manager owner selection.
    11. The source-of-truth selection on `/tenant/projects/new` contains tenant sources of truth only.
    12. Submission of valid values on `/tenant/projects/new` with a tenant-superuser session redirects to `/tenant/projects`.
    13. Browser view of `/tenant/projects` after a valid tenant-superuser submission shows one project list row with the submitted job title and the selected Hiring Manager owner.
    14. Submission of valid values on `/tenant/projects/new` with a Hiring Manager session redirects to `/tenant/projects`.
    15. Browser view of `/tenant/projects` after a valid Hiring Manager submission shows one project list row with the submitted job title and the signed-in Hiring Manager as owner.
    16. Submission of `/tenant/projects/new` with a tenant-superuser session and without a selected Hiring Manager owner remains on `/tenant/projects/new`.
    17. The route `/tenant/projects/{projectId}` is reachable by activating a `View project` action on `/tenant/projects`.
    18. Browser view of `/tenant/projects/{projectId}` shows the project job title, the project department, one Hiring Manager owner, and the selected source-of-truth name.
    19. Submission of a form with a missing required value on `/tenant/projects/new` remains on `/tenant/projects/new`.
    20. A request to `/tenant/projects/new` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`.
    21. A request to `/tenant/projects/{projectId}` for a project from another tenant shows the text `Project not found`.
    22. A request to `/tenant/projects/{projectId}` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`.
    23. A request to `/tenant/projects` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the `Projects` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/projects`**
    2. **AT2 - Browser activation of the `Projects` action on `/tenant/dashboard` with a Hiring Manager session opens `/tenant/projects`**
    3. **AT3 - Browser view of `/tenant/projects` with two tenant recruitment projects and one different-tenant recruitment project shows two project list rows**
    4. **AT4 - Browser view of `/tenant/projects` with a Hiring Manager session shows only project list rows owned by the signed-in Hiring Manager**
    5. **AT5 - Browser activation of the `Create project` action on `/tenant/projects` with a tenant-superuser session opens `/tenant/projects/new`**
    6. **AT6 - Browser activation of the `Create project` action on `/tenant/projects` with a Hiring Manager session opens `/tenant/projects/new`**
    7. **AT7 - Browser view of `/tenant/projects/new` with a tenant-superuser session shows a job title input, a department input, a Hiring Manager owner selection, a source-of-truth selection, and a submit action**
    8. **AT8 - Browser view of the Hiring Manager owner selection on `/tenant/projects/new` with a tenant-superuser session shows tenant Hiring Manager users and does not show tenant-superuser users or different-tenant users**
    9. **AT9 - Browser view of `/tenant/projects/new` with a Hiring Manager session shows a job title input, a department input, a source-of-truth selection, and a submit action**
    10. **AT10 - Browser view of `/tenant/projects/new` with a Hiring Manager session does not show a Hiring Manager owner selection**
    11. **AT11 - Browser view of the source-of-truth selection on `/tenant/projects/new` shows tenant sources of truth and does not show different-tenant sources of truth**
    12. **AT12 - Browser submission of valid values on `/tenant/projects/new` with a tenant-superuser session redirects to `/tenant/projects`**
    13. **AT13 - Browser view of `/tenant/projects` after a valid tenant-superuser submission shows one project list row with the submitted job title and the selected Hiring Manager owner**
    14. **AT14 - Browser submission of valid values on `/tenant/projects/new` with a Hiring Manager session redirects to `/tenant/projects`**
    15. **AT15 - Browser view of `/tenant/projects` after a valid Hiring Manager submission shows one project list row with the submitted job title and the signed-in Hiring Manager as owner**
    16. **AT16 - Browser submission of `/tenant/projects/new` with a tenant-superuser session and without a selected Hiring Manager owner remains on `/tenant/projects/new`**
    17. **AT17 - Browser activation of a `View project` action on `/tenant/projects` opens `/tenant/projects/{projectId}`**
    18. **AT18 - Browser view of `/tenant/projects/{projectId}` shows the project job title, the project department, one Hiring Manager owner, and the selected source-of-truth name**
    19. **AT19 - Browser submission of a form with a missing required value on `/tenant/projects/new` remains on `/tenant/projects/new`**
    20. **AT20 - Browser navigation to `/tenant/projects/new` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`**
    21. **AT21 - Browser navigation to `/tenant/projects/{projectId}` for a project from another tenant shows the text `Project not found`**
    22. **AT22 - Browser navigation to `/tenant/projects/{projectId}` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`**
    23. **AT23 - Browser navigation to `/tenant/projects` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Multiple source-of-truth selection. Ideal Candidate Profile generation. Candidate scoring input orchestration. Project editing. Project closure. Multiple owners per project.
  - **Status:** Completed 2026-03-25.

- [x] **SIP-3.5: Tenant bootstrap flow completes from workspace entry to owned project access**
  - **Summary:** The tenant workspace allows a tenant superuser to complete the first tenant bootstrap flow and allows the created Hiring Manager to reach the created project.
  - **Objective (WHY):** The epic requires one browser-verifiable tenant workflow that proves the tenant workspace is usable end to end.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1, SIP-3.2, SIP-3.3, SIP-3.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. A tenant-superuser session on `/tenant/dashboard` can reach `/tenant/users` by activating the `Users` action.
    2. A tenant-superuser session can create one Hiring Manager user from `/tenant/users/new`.
    3. A tenant-superuser session on `/tenant/dashboard` can reach `/tenant/sots` by activating the `Sources of truth` action.
    4. A tenant-superuser session can create one tenant source of truth from `/tenant/sots/new`.
    5. A tenant-superuser session on `/tenant/dashboard` can reach `/tenant/projects` by activating the `Projects` action.
    6. A tenant-superuser session can create one recruitment project from `/tenant/projects/new` by selecting the created Hiring Manager user and the created source of truth.
    7. Browser view of `/tenant/projects/{projectId}` after the created project is opened shows the created project title, the created Hiring Manager owner, and the created source-of-truth name.
    8. A created Hiring Manager session on `/sign-in` redirects to `/tenant/dashboard`.
    9. A created Hiring Manager session can reach `/tenant/projects` by activating the `Projects` action on `/tenant/dashboard`.
    10. Browser view of `/tenant/projects` with the created Hiring Manager session shows the created project list row.
    11. A created Hiring Manager session can open the created project from `/tenant/projects` by activating a `View project` action.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the `Users` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/users`**
    2. **AT2 - Browser submission of valid values on `/tenant/users/new` with a tenant-superuser session creates one Hiring Manager user**
    3. **AT3 - Browser activation of the `Sources of truth` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/sots`**
    4. **AT4 - Browser submission of valid values on `/tenant/sots/new` with a tenant-superuser session creates one tenant source of truth**
    5. **AT5 - Browser activation of the `Projects` action on `/tenant/dashboard` with a tenant-superuser session opens `/tenant/projects`**
    6. **AT6 - Browser submission of valid values on `/tenant/projects/new` with a tenant-superuser session creates one recruitment project with the created Hiring Manager owner and the created source of truth**
    7. **AT7 - Browser view of `/tenant/projects/{projectId}` after activation of a `View project` action shows the created project title, the created Hiring Manager owner, and the created source-of-truth name**
    8. **AT8 - Browser submission of the created Hiring Manager credentials on `/sign-in` redirects to `/tenant/dashboard`**
    9. **AT9 - Browser activation of the `Projects` action on `/tenant/dashboard` with the created Hiring Manager session opens `/tenant/projects`**
    10. **AT10 - Browser view of `/tenant/projects` with the created Hiring Manager session shows the created project list row**
    11. **AT11 - Browser activation of a `View project` action on `/tenant/projects` with the created Hiring Manager session opens `/tenant/projects/{projectId}`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Ideal Candidate Profile definition. Interview flow definition. Candidate-facing entry points. Leaderboard access.
  - **Status:** Completed 2026-03-25.

### **EPIC-4: Navigation and Workflow Recovery**
- **Category:** USER-FACING
- *Objective: Establish recoverable in-app navigation across the current authenticated Admin and Tenant workflows so users can move between workspace roots, section pages, create pages, and detail pages without guessing URLs or relying on browser history.*

- [x] **SIP-4.1: Global Admin pages expose visible return navigation**
  - **Summary:** The routes `/admin/dashboard`, `/admin/tenants/new`, and `/admin/tenants/{tenantId}` expose visible in-app navigation between the Admin workspace root and tenant subpages.
  - **Objective (WHY):** The Global Admin must be able to move forward and backward across the current tenant bootstrap workflow without losing orientation.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.2, SIP-1.3, SIP-1.5.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. Browser view of `/admin/dashboard` with an authenticated Global Admin session displays a visible current-location label for the Admin workspace.
    2. Browser view of `/admin/dashboard` with an authenticated Global Admin session displays a visible navigation action to `/admin/tenants/new`.
    3. Browser view of `/admin/tenants/new` with an authenticated Global Admin session displays a visible `Admin dashboard` return action that opens `/admin/dashboard`.
    4. Browser view of `/admin/tenants/{tenantId}` with an authenticated Global Admin session displays a visible `Admin dashboard` return action that opens `/admin/dashboard`.
    5. Browser view of `/admin/tenants/{tenantId}` with an authenticated Global Admin session displays a visible `Back to tenants` action that opens `/admin/dashboard`.
    6. A request to `/admin/tenants/new` without an authenticated Global Admin session redirects to `/sign-in`.
    7. A request to `/admin/tenants/{tenantId}` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/admin/dashboard` with an authenticated Global Admin session shows a visible current-location label for the Admin workspace**
    2. **AT2 - Browser activation of the visible navigation action to `/admin/tenants/new` on `/admin/dashboard` opens `/admin/tenants/new`**
    3. **AT3 - Browser activation of the visible `Admin dashboard` return action on `/admin/tenants/new` opens `/admin/dashboard`**
    4. **AT4 - Browser activation of the visible `Admin dashboard` return action on `/admin/tenants/{tenantId}` opens `/admin/dashboard`**
    5. **AT5 - Browser activation of the visible `Back to tenants` action on `/admin/tenants/{tenantId}` opens `/admin/dashboard`**
    6. **AT6 - Browser navigation to `/admin/tenants/new` without an authenticated Global Admin session redirects to `/sign-in`**
    7. **AT7 - Browser navigation to `/admin/tenants/{tenantId}` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant editing. Tenant deletion. Global Admin analytics. Cross-tenant impersonation.
  - **Status:** Completed 2026-03-26.

- [x] **SIP-4.2: Tenant superuser pages expose section navigation and return paths**
  - **Summary:** The routes `/tenant/dashboard`, `/tenant/users`, `/tenant/users/new`, `/tenant/sots`, `/tenant/sots/new`, `/tenant/sots/{sotId}`, `/tenant/projects`, `/tenant/projects/new`, and `/tenant/projects/{projectId}` expose visible in-app section navigation and visible return paths for tenant-superuser workflows.
  - **Objective (WHY):** Tenant superusers need continuous orientation and recovery across users, sources of truth, and projects after the first tenant bootstrap has been completed.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1, SIP-3.2, SIP-3.3, SIP-3.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. Browser view of `/tenant/dashboard` with a tenant-superuser session displays visible navigation actions to `/tenant/users`, `/tenant/sots`, and `/tenant/projects`.
    2. Browser view of `/tenant/users` with a tenant-superuser session displays visible navigation actions to `/tenant/dashboard`, `/tenant/sots`, and `/tenant/projects`.
    3. Browser view of `/tenant/sots` with a tenant-superuser session displays visible navigation actions to `/tenant/dashboard`, `/tenant/users`, and `/tenant/projects`.
    4. Browser view of `/tenant/projects` with a tenant-superuser session displays visible navigation actions to `/tenant/dashboard`, `/tenant/users`, and `/tenant/sots`.
    5. Browser view of `/tenant/users/new` with a tenant-superuser session displays a visible `Tenant dashboard` return action and a visible `Users` return action.
    6. Browser view of `/tenant/sots/new` with a tenant-superuser session displays a visible `Tenant dashboard` return action and a visible `Sources of truth` return action.
    7. Browser view of `/tenant/sots/{sotId}` with a tenant-superuser session displays a visible `Tenant dashboard` return action and a visible `Sources of truth` return action.
    8. Browser view of `/tenant/projects/new` with a tenant-superuser session displays a visible `Tenant dashboard` return action and a visible `Projects` return action.
    9. Browser view of `/tenant/projects/{projectId}` with a tenant-superuser session displays a visible `Tenant dashboard` return action and a visible `Projects` return action.
    10. A request to `/tenant/users/new` without an authenticated tenant-superuser session redirects to `/sign-in`.
    11. A request to `/tenant/sots/new` without an authenticated tenant-superuser session redirects to `/sign-in`.
    12. A request to `/tenant/projects/new` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/tenant/dashboard` with a tenant-superuser session shows visible navigation actions to `/tenant/users`, `/tenant/sots`, and `/tenant/projects`**
    2. **AT2 - Browser view of `/tenant/users` with a tenant-superuser session shows visible navigation actions to `/tenant/dashboard`, `/tenant/sots`, and `/tenant/projects`**
    3. **AT3 - Browser view of `/tenant/sots` with a tenant-superuser session shows visible navigation actions to `/tenant/dashboard`, `/tenant/users`, and `/tenant/projects`**
    4. **AT4 - Browser view of `/tenant/projects` with a tenant-superuser session shows visible navigation actions to `/tenant/dashboard`, `/tenant/users`, and `/tenant/sots`**
    5. **AT5 - Browser view of `/tenant/users/new` with a tenant-superuser session shows a visible `Tenant dashboard` return action and a visible `Users` return action**
    6. **AT6 - Browser view of `/tenant/sots/new` with a tenant-superuser session shows a visible `Tenant dashboard` return action and a visible `Sources of truth` return action**
    7. **AT7 - Browser view of `/tenant/sots/{sotId}` with a tenant-superuser session shows a visible `Tenant dashboard` return action and a visible `Sources of truth` return action**
    8. **AT8 - Browser view of `/tenant/projects/new` with a tenant-superuser session shows a visible `Tenant dashboard` return action and a visible `Projects` return action**
    9. **AT9 - Browser view of `/tenant/projects/{projectId}` with a tenant-superuser session shows a visible `Tenant dashboard` return action and a visible `Projects` return action**
    10. **AT10 - Browser navigation to `/tenant/users/new` without an authenticated tenant-superuser session redirects to `/sign-in`**
    11. **AT11 - Browser navigation to `/tenant/sots/new` without an authenticated tenant-superuser session redirects to `/sign-in`**
    12. **AT12 - Browser navigation to `/tenant/projects/new` without an authenticated tenant-superuser or Hiring Manager session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** User editing. Source-of-truth editing. Project editing. Ideal Candidate Profile definition.
  - **Status:** Completed 2026-03-26.

- [x] **SIP-4.3: Hiring Manager pages expose role-scoped return navigation**
  - **Summary:** The routes `/tenant/dashboard`, `/tenant/projects`, `/tenant/projects/new`, and `/tenant/projects/{projectId}` expose visible in-app navigation for Hiring Manager workflows without exposing tenant-superuser sections.
  - **Objective (WHY):** Hiring Managers need recoverable navigation across their project workflow while remaining inside role-scoped boundaries.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.1, SIP-3.4, SIP-3.5.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. Browser view of `/tenant/dashboard` with a Hiring Manager session displays a visible navigation action to `/tenant/projects`.
    2. Browser view of `/tenant/projects` with a Hiring Manager session displays a visible navigation action to `/tenant/dashboard`.
    3. Browser view of `/tenant/projects/new` with a Hiring Manager session displays a visible `Tenant dashboard` return action and a visible `Projects` return action.
    4. Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session displays a visible `Tenant dashboard` return action and a visible `Projects` return action.
    5. Browser view of `/tenant/projects` with a Hiring Manager session does not display a visible navigation action to `/tenant/users`.
    6. Browser view of `/tenant/projects` with a Hiring Manager session does not display a visible navigation action to `/tenant/sots`.
    7. Browser view of `/tenant/projects/new` with a Hiring Manager session does not display a visible navigation action to `/tenant/users`.
    8. Browser view of `/tenant/projects/new` with a Hiring Manager session does not display a visible navigation action to `/tenant/sots`.
    9. Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session does not display a visible navigation action to `/tenant/users`.
    10. Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session does not display a visible navigation action to `/tenant/sots`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/tenant/dashboard` with a Hiring Manager session shows a visible navigation action to `/tenant/projects`**
    2. **AT2 - Browser view of `/tenant/projects` with a Hiring Manager session shows a visible navigation action to `/tenant/dashboard`**
    3. **AT3 - Browser view of `/tenant/projects/new` with a Hiring Manager session shows a visible `Tenant dashboard` return action and a visible `Projects` return action**
    4. **AT4 - Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session shows a visible `Tenant dashboard` return action and a visible `Projects` return action**
    5. **AT5 - Browser view of `/tenant/projects` with a Hiring Manager session does not show a visible navigation action to `/tenant/users`**
    6. **AT6 - Browser view of `/tenant/projects` with a Hiring Manager session does not show a visible navigation action to `/tenant/sots`**
    7. **AT7 - Browser view of `/tenant/projects/new` with a Hiring Manager session does not show a visible navigation action to `/tenant/users`**
    8. **AT8 - Browser view of `/tenant/projects/new` with a Hiring Manager session does not show a visible navigation action to `/tenant/sots`**
    9. **AT9 - Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session does not show a visible navigation action to `/tenant/users`**
    10. **AT10 - Browser view of `/tenant/projects/{projectId}` with a Hiring Manager session does not show a visible navigation action to `/tenant/sots`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Tenant-superuser navigation. Hiring Manager access to users. Hiring Manager access to sources of truth. Project editing.
  - **Status:** Completed 2026-03-26.

- [x] **SIP-4.4: Navigation recovery completes across Admin and Tenant workflows**
  - **Summary:** The current Admin and Tenant workflows support visible round-trip navigation from workspace roots to create pages and detail pages and back again.
  - **Objective (WHY):** The current delivered workflows need one end-to-end proof that the user can recover orientation and return paths without relying on browser history.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-4.1, SIP-4.2, SIP-4.3.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. A Global Admin session can navigate from `/admin/dashboard` to `/admin/tenants/new` and back to `/admin/dashboard` by activating visible in-app navigation actions only.
    2. A Global Admin session can navigate from `/admin/dashboard` to `/admin/tenants/{tenantId}` and back to `/admin/dashboard` by activating visible in-app navigation actions only.
    3. A tenant-superuser session can navigate from `/tenant/dashboard` to `/tenant/users`, to `/tenant/users/new`, back to `/tenant/users`, and back to `/tenant/dashboard` by activating visible in-app navigation actions only.
    4. A tenant-superuser session can navigate from `/tenant/dashboard` to `/tenant/sots`, to `/tenant/sots/new`, back to `/tenant/sots`, and back to `/tenant/dashboard` by activating visible in-app navigation actions only.
    5. A tenant-superuser session can navigate from `/tenant/dashboard` to `/tenant/projects`, to `/tenant/projects/new`, back to `/tenant/projects`, and back to `/tenant/dashboard` by activating visible in-app navigation actions only.
    6. A tenant-superuser session can navigate from `/tenant/projects` to `/tenant/projects/{projectId}`, back to `/tenant/projects`, and back to `/tenant/dashboard` by activating visible in-app navigation actions only.
    7. A Hiring Manager session can navigate from `/tenant/dashboard` to `/tenant/projects`, to `/tenant/projects/{projectId}`, back to `/tenant/projects`, and back to `/tenant/dashboard` by activating visible in-app navigation actions only.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser workflow with a Global Admin session navigates from `/admin/dashboard` to `/admin/tenants/new` and returns to `/admin/dashboard` by activating visible in-app navigation actions only**
    2. **AT2 - Browser workflow with a Global Admin session navigates from `/admin/dashboard` to `/admin/tenants/{tenantId}` and returns to `/admin/dashboard` by activating visible in-app navigation actions only**
    3. **AT3 - Browser workflow with a tenant-superuser session navigates from `/tenant/dashboard` to `/tenant/users`, to `/tenant/users/new`, returns to `/tenant/users`, and returns to `/tenant/dashboard` by activating visible in-app navigation actions only**
    4. **AT4 - Browser workflow with a tenant-superuser session navigates from `/tenant/dashboard` to `/tenant/sots`, to `/tenant/sots/new`, returns to `/tenant/sots`, and returns to `/tenant/dashboard` by activating visible in-app navigation actions only**
    5. **AT5 - Browser workflow with a tenant-superuser session navigates from `/tenant/dashboard` to `/tenant/projects`, to `/tenant/projects/new`, returns to `/tenant/projects`, and returns to `/tenant/dashboard` by activating visible in-app navigation actions only**
    6. **AT6 - Browser workflow with a tenant-superuser session navigates from `/tenant/projects` to `/tenant/projects/{projectId}`, returns to `/tenant/projects`, and returns to `/tenant/dashboard` by activating visible in-app navigation actions only**
    7. **AT7 - Browser workflow with a Hiring Manager session navigates from `/tenant/dashboard` to `/tenant/projects`, to `/tenant/projects/{projectId}`, returns to `/tenant/projects`, and returns to `/tenant/dashboard` by activating visible in-app navigation actions only**
  - **Explicit Non-Goals (OUT OF SCOPE):** Browser-history support. Candidate-facing navigation. New domain workflows. Global redesign of authenticated layouts outside the current pages.
  - **Status:** Completed 2026-03-26.

### **EPIC-5: AI Governance, Cost Attribution, SoT Ingestion, and Candidate Designer Bootstrap**
- **Category:** USER-FACING
- *Objective: Establish the first governed AI workflow by allowing Global Admins to control which AI models are available and inspect attributable AI usage and cost, allowing tenant superusers to upload text files and save AI-generated sources of truth, and allowing Hiring Managers to use selected sources of truth to generate and save project ICPs.*

- [ ] **SIP-5.1: Global Admin can configure available AI models for governed workflows**
  - **Summary:** The route `/admin/ai` allows a Global Admin to enable and disable AI models for source-of-truth ingestion and Candidate Designer workflows.
  - **Objective (WHY):** AI execution must be governed by Global Admins before tenant and project workflows depend on model availability.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-1.2, SIP-4.1.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/admin/ai` is reachable by activating a visible `AI governance` action on `/admin/dashboard`.
    2. Browser view of `/admin/ai` with an authenticated Global Admin session displays one model list row per available AI model.
    3. Each model list row on `/admin/ai` displays one availability control for `Source of truth ingestion`.
    4. Each model list row on `/admin/ai` displays one availability control for `Candidate Designer`.
    5. Submission of a valid availability change on `/admin/ai` remains on `/admin/ai`.
    6. Browser revisit of `/admin/ai` after a valid availability change shows the saved model availability state.
    7. A model disabled for `Source of truth ingestion` does not appear as a selectable model on `/tenant/sots/upload`.
    8. A model disabled for `Candidate Designer` does not appear as a selectable model on `/tenant/projects/{projectId}/candidate-designer`.
    9. Browser view of `/admin/ai` with an authenticated Global Admin session displays a visible `Back to dashboard` action that opens `/admin/dashboard`.
    10. A request to `/admin/ai` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the visible `AI governance` action on `/admin/dashboard` opens `/admin/ai`**
    2. **AT2 - Browser view of `/admin/ai` with an authenticated Global Admin session shows one model list row per available AI model**
    3. **AT3 - Browser view of one model list row on `/admin/ai` shows one availability control for `Source of truth ingestion`**
    4. **AT4 - Browser view of one model list row on `/admin/ai` shows one availability control for `Candidate Designer`**
    5. **AT5 - Browser submission of a valid availability change on `/admin/ai` remains on `/admin/ai`**
    6. **AT6 - Browser revisit of `/admin/ai` after a valid availability change shows the saved model availability state**
    7. **AT7 - Browser view of `/tenant/sots/upload` after a model is disabled for `Source of truth ingestion` does not show that model as a selectable model**
    8. **AT8 - Browser view of `/tenant/projects/{projectId}/candidate-designer` after a model is disabled for `Candidate Designer` does not show that model as a selectable model**
    9. **AT9 - Browser activation of the visible `Back to dashboard` action on `/admin/ai` opens `/admin/dashboard`**
    10. **AT10 - Browser navigation to `/admin/ai` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Provider failover policy. Prompt editing. Tenant-specific model overrides. Candidate-facing AI configuration.
  - **Status:** Planned 2026-03-26.

- [ ] **SIP-5.2: Global Admin can inspect attributable AI usage and cost**
  - **Summary:** The route `/admin/ai` displays attributable AI usage and cost for governed workflows by period, model, tenant, and project.
  - **Objective (WHY):** AI token spend must be attributable to workflow usage so the platform can control expenses and support billing evidence.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-5.1, SIP-5.3, SIP-5.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. Browser view of `/admin/ai` with an authenticated Global Admin session displays daily totals for AI request count, input tokens, output tokens, total tokens, and estimated cost.
    2. Browser view of `/admin/ai` with an authenticated Global Admin session displays weekly totals for AI request count, input tokens, output tokens, total tokens, and estimated cost.
    3. Browser view of `/admin/ai` with an authenticated Global Admin session displays monthly totals for AI request count, input tokens, output tokens, total tokens, and estimated cost.
    4. Browser view of `/admin/ai` with an authenticated Global Admin session displays one visible trendline for the selected reporting period.
    5. Browser view of `/admin/ai` with an authenticated Global Admin session displays one usage list row per AI request in the filtered result.
    6. Each usage list row on `/admin/ai` displays tenant identifier, project identifier or `None`, workflow name, model name, request timestamp, input tokens, output tokens, total tokens, and estimated cost.
    7. Browser view of `/admin/ai` allows filtering the usage list by tenant, project, workflow, and model.
    8. Browser view of `/admin/ai` after one source-of-truth ingestion request shows one usage list row with the workflow name `Source of truth ingestion`.
    9. Browser view of `/admin/ai` after one Candidate Designer request shows one usage list row with the workflow name `Candidate Designer`.
    10. A request to `/admin/ai` without an authenticated Global Admin session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser view of `/admin/ai` with an authenticated Global Admin session shows daily totals for AI request count, input tokens, output tokens, total tokens, and estimated cost**
    2. **AT2 - Browser view of `/admin/ai` with an authenticated Global Admin session shows weekly totals for AI request count, input tokens, output tokens, total tokens, and estimated cost**
    3. **AT3 - Browser view of `/admin/ai` with an authenticated Global Admin session shows monthly totals for AI request count, input tokens, output tokens, total tokens, and estimated cost**
    4. **AT4 - Browser view of `/admin/ai` with an authenticated Global Admin session shows one visible trendline for the selected reporting period**
    5. **AT5 - Browser view of one usage list row on `/admin/ai` shows tenant identifier, project identifier or `None`, workflow name, model name, request timestamp, input tokens, output tokens, total tokens, and estimated cost**
    6. **AT6 - Browser activation of tenant, project, workflow, and model filters on `/admin/ai` filters the usage list**
    7. **AT7 - Browser view of `/admin/ai` after one source-of-truth ingestion request shows one usage list row with the workflow name `Source of truth ingestion`**
    8. **AT8 - Browser view of `/admin/ai` after one Candidate Designer request shows one usage list row with the workflow name `Candidate Designer`**
    9. **AT9 - Browser navigation to `/admin/ai` without an authenticated Global Admin session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Invoice generation. Payment collection. Tenant-facing AI cost dashboards. Non-AI platform cost reporting.
  - **Status:** Planned 2026-03-26.

- [ ] **SIP-5.3: Tenant superuser can upload text and save an AI-generated source of truth**
  - **Summary:** The route `/tenant/sots/upload` allows a tenant superuser to upload a text file, review an AI-generated source-of-truth draft, and save it as a tenant-scoped source of truth.
  - **Objective (WHY):** Manual source-of-truth entry must be supplemented by a usable ingestion path before Candidate Designer can rely on tenant context at scale.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.3, SIP-4.2, SIP-5.1.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/tenant/sots/upload` is reachable by activating a visible `Upload source material` action on `/tenant/sots` with a tenant-superuser session.
    2. Browser view of `/tenant/sots/upload` with a tenant-superuser session displays a file-upload control, a topic input, a name input, a model selection, a `Generate draft` action, and a `Save source of truth` action.
    3. The model selection on `/tenant/sots/upload` displays AI models enabled for `Source of truth ingestion` only.
    4. Submission of a valid text file, topic value, name value, and model value on `/tenant/sots/upload` remains on `/tenant/sots/upload`.
    5. Browser view of `/tenant/sots/upload` after a valid generation request shows one draft source of truth with topic, name, schema version, and one entry list row per generated entry.
    6. Each generated entry list row on `/tenant/sots/upload` displays key, label, value type, and value.
    7. Submission of a valid generated draft on `/tenant/sots/upload` redirects to `/tenant/sots`.
    8. Browser view of `/tenant/sots` after a valid generated draft is saved shows one source-of-truth list row with the generated topic and generated name.
    9. Browser view of `/tenant/sots/upload` with a tenant-superuser session displays a visible `Back to sources of truth` action that opens `/tenant/sots`.
    10. Submission of `/tenant/sots/upload` with a missing file remains on `/tenant/sots/upload`.
    11. A request to `/tenant/sots/upload` with an authenticated Hiring Manager session redirects to `/sign-in`.
    12. A request to `/tenant/sots/upload` without an authenticated session redirects to `/sign-in`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the visible `Upload source material` action on `/tenant/sots` with a tenant-superuser session opens `/tenant/sots/upload`**
    2. **AT2 - Browser view of `/tenant/sots/upload` with a tenant-superuser session shows a file-upload control, a topic input, a name input, a model selection, a `Generate draft` action, and a `Save source of truth` action**
    3. **AT3 - Browser view of the model selection on `/tenant/sots/upload` shows AI models enabled for `Source of truth ingestion` only**
    4. **AT4 - Browser submission of a valid text file, topic value, name value, and model value on `/tenant/sots/upload` remains on `/tenant/sots/upload`**
    5. **AT5 - Browser view of `/tenant/sots/upload` after a valid generation request shows one draft source of truth with topic, name, schema version, and one entry list row per generated entry**
    6. **AT6 - Browser view of one generated entry list row on `/tenant/sots/upload` shows key, label, value type, and value**
    7. **AT7 - Browser submission of a valid generated draft on `/tenant/sots/upload` redirects to `/tenant/sots`**
    8. **AT8 - Browser view of `/tenant/sots` after a valid generated draft is saved shows one source-of-truth list row with the generated topic and generated name**
    9. **AT9 - Browser activation of the visible `Back to sources of truth` action on `/tenant/sots/upload` opens `/tenant/sots`**
    10. **AT10 - Browser submission of `/tenant/sots/upload` with a missing file remains on `/tenant/sots/upload`**
    11. **AT11 - Browser navigation to `/tenant/sots/upload` with an authenticated Hiring Manager session redirects to `/sign-in`**
    12. **AT12 - Browser navigation to `/tenant/sots/upload` without an authenticated session redirects to `/sign-in`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Source-of-truth editing after generation. Non-text file formats. Multi-file ingestion. Topic-specific source-of-truth templates.
  - **Status:** Planned 2026-03-26.

- [ ] **SIP-5.4: Hiring Manager can generate and save an ICP from selected sources of truth**
  - **Summary:** The routes `/tenant/projects/{projectId}/candidate-designer` and `/tenant/projects/{projectId}/icp` allow the project owner Hiring Manager to generate and save an ICP from selected sources of truth.
  - **Objective (WHY):** The project owner must be able to produce the project ICP before downstream candidate-facing and evaluation workflows can begin.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-3.4, SIP-4.3, SIP-5.1, SIP-5.3.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. The route `/tenant/projects/{projectId}/candidate-designer` is reachable by activating a visible `Define ICP` action on `/tenant/projects/{projectId}` with the project owner Hiring Manager session.
    2. Browser view of `/tenant/projects/{projectId}/candidate-designer` with the project owner Hiring Manager session displays the project title, a source-of-truth multiselection, a model selection, a design prompt input, a `Generate ICP` action, and a `Save ICP` action.
    3. The source-of-truth multiselection on `/tenant/projects/{projectId}/candidate-designer` displays tenant sources of truth only.
    4. The model selection on `/tenant/projects/{projectId}/candidate-designer` displays AI models enabled for `Candidate Designer` only.
    5. Submission of valid selected sources of truth, model value, and design prompt value on `/tenant/projects/{projectId}/candidate-designer` remains on `/tenant/projects/{projectId}/candidate-designer`.
    6. Browser view of `/tenant/projects/{projectId}/candidate-designer` after a valid generation request shows one draft ICP with required competencies, experience expectations, success criteria, personal traits, and evaluation parameters.
    7. Submission of a valid draft ICP on `/tenant/projects/{projectId}/candidate-designer` redirects to `/tenant/projects/{projectId}`.
    8. Browser view of `/tenant/projects/{projectId}` after a valid ICP save shows the text `ICP status: Defined`.
    9. Browser view of `/tenant/projects/{projectId}` after a valid ICP save displays a visible `View ICP` action that opens `/tenant/projects/{projectId}/icp`.
    10. Browser view of `/tenant/projects/{projectId}/icp` shows required competencies, experience expectations, success criteria, personal traits, and evaluation parameters from the saved ICP.
    11. Browser view of `/tenant/projects/{projectId}/candidate-designer` with the project owner Hiring Manager session displays a visible `Back to project` action that opens `/tenant/projects/{projectId}`.
    12. Browser view of `/tenant/projects/{projectId}/icp` with the project owner Hiring Manager session displays a visible `Back to project` action that opens `/tenant/projects/{projectId}`.
    13. Submission of `/tenant/projects/{projectId}/candidate-designer` with no selected source of truth remains on `/tenant/projects/{projectId}/candidate-designer`.
    14. A request to `/tenant/projects/{projectId}/candidate-designer` without an authenticated session redirects to `/sign-in`.
    15. A request to `/tenant/projects/{projectId}/candidate-designer` with a tenant-superuser session redirects to `/sign-in`.
    16. A request to `/tenant/projects/{projectId}/candidate-designer` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`.
    17. A request to `/tenant/projects/{projectId}/icp` without an authenticated session redirects to `/sign-in`.
    18. A request to `/tenant/projects/{projectId}/icp` with a tenant-superuser session redirects to `/sign-in`.
    19. A request to `/tenant/projects/{projectId}/icp` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the visible `Define ICP` action on `/tenant/projects/{projectId}` with the project owner Hiring Manager session opens `/tenant/projects/{projectId}/candidate-designer`**
    2. **AT2 - Browser view of `/tenant/projects/{projectId}/candidate-designer` with the project owner Hiring Manager session shows the project title, a source-of-truth multiselection, a model selection, a design prompt input, a `Generate ICP` action, and a `Save ICP` action**
    3. **AT3 - Browser view of the source-of-truth multiselection on `/tenant/projects/{projectId}/candidate-designer` shows tenant sources of truth only**
    4. **AT4 - Browser view of the model selection on `/tenant/projects/{projectId}/candidate-designer` shows AI models enabled for `Candidate Designer` only**
    5. **AT5 - Browser submission of valid selected sources of truth, model value, and design prompt value on `/tenant/projects/{projectId}/candidate-designer` remains on `/tenant/projects/{projectId}/candidate-designer`**
    6. **AT6 - Browser view of `/tenant/projects/{projectId}/candidate-designer` after a valid generation request shows one draft ICP with required competencies, experience expectations, success criteria, personal traits, and evaluation parameters**
    7. **AT7 - Browser submission of a valid draft ICP on `/tenant/projects/{projectId}/candidate-designer` redirects to `/tenant/projects/{projectId}`**
    8. **AT8 - Browser view of `/tenant/projects/{projectId}` after a valid ICP save shows the text `ICP status: Defined`**
    9. **AT9 - Browser activation of the visible `View ICP` action on `/tenant/projects/{projectId}` opens `/tenant/projects/{projectId}/icp`**
    10. **AT10 - Browser view of `/tenant/projects/{projectId}/icp` shows required competencies, experience expectations, success criteria, personal traits, and evaluation parameters from the saved ICP**
    11. **AT11 - Browser activation of the visible `Back to project` action on `/tenant/projects/{projectId}/candidate-designer` opens `/tenant/projects/{projectId}`**
    12. **AT12 - Browser activation of the visible `Back to project` action on `/tenant/projects/{projectId}/icp` opens `/tenant/projects/{projectId}`**
    13. **AT13 - Browser submission of `/tenant/projects/{projectId}/candidate-designer` with no selected source of truth remains on `/tenant/projects/{projectId}/candidate-designer`**
    14. **AT14 - Browser navigation to `/tenant/projects/{projectId}/candidate-designer` without an authenticated session redirects to `/sign-in`**
    15. **AT15 - Browser navigation to `/tenant/projects/{projectId}/candidate-designer` with a tenant-superuser session redirects to `/sign-in`**
    16. **AT16 - Browser navigation to `/tenant/projects/{projectId}/candidate-designer` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`**
    17. **AT17 - Browser navigation to `/tenant/projects/{projectId}/icp` without an authenticated session redirects to `/sign-in`**
    18. **AT18 - Browser navigation to `/tenant/projects/{projectId}/icp` with a tenant-superuser session redirects to `/sign-in`**
    19. **AT19 - Browser navigation to `/tenant/projects/{projectId}/icp` with a Hiring Manager session for a project not owned by the signed-in Hiring Manager shows the text `Project not found`**
  - **Explicit Non-Goals (OUT OF SCOPE):** Interactive Job Post generation. Interview flow definition. Candidate registration. Candidate scoring.
  - **Status:** Planned 2026-03-26.

- [ ] **SIP-5.5: Governed AI workflow completes from model control to attributable ICP generation**
  - **Summary:** The governed AI path supports model control, attributable source-of-truth ingestion, attributable ICP generation, and saved ICP access.
  - **Objective (WHY):** The epic requires one end-to-end proof that governed AI execution produces usable recruitment design output and attributable cost evidence.
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** SIP-5.1, SIP-5.2, SIP-5.3, SIP-5.4.
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. A Global Admin session can open `/admin/ai` by activating the visible `AI governance` action on `/admin/dashboard`.
    2. A Global Admin session can enable one AI model for `Source of truth ingestion` on `/admin/ai`.
    3. A Global Admin session can enable one AI model for `Candidate Designer` on `/admin/ai`.
    4. A tenant-superuser session can open `/tenant/sots/upload` by activating the visible `Upload source material` action on `/tenant/sots`.
    5. A tenant-superuser session can submit one valid text file, topic value, name value, and model value on `/tenant/sots/upload`.
    6. Browser view of `/tenant/sots/upload` after the valid generation request shows one draft source of truth.
    7. A tenant-superuser session can save the generated source-of-truth draft on `/tenant/sots/upload`.
    8. A project owner Hiring Manager session can open `/tenant/projects/{projectId}/candidate-designer` by activating the visible `Define ICP` action on `/tenant/projects/{projectId}`.
    9. A project owner Hiring Manager session can submit valid selected sources of truth, model value, and design prompt value on `/tenant/projects/{projectId}/candidate-designer`.
    10. Browser view of `/tenant/projects/{projectId}/candidate-designer` after the valid generation request shows one draft ICP.
    11. A project owner Hiring Manager session can save the generated ICP draft on `/tenant/projects/{projectId}/candidate-designer`.
    12. A project owner Hiring Manager session can open `/tenant/projects/{projectId}/icp` by activating the visible `View ICP` action on `/tenant/projects/{projectId}` after the ICP is saved.
    13. Browser view of `/tenant/projects/{projectId}/icp` after the saved ICP is opened shows the saved ICP content.
    14. Browser view of `/admin/ai` after the source-of-truth ingestion request shows one usage list row attributed to the tenant and the workflow `Source of truth ingestion`.
    15. The usage list row for `Source of truth ingestion` on `/admin/ai` displays total tokens.
    16. The usage list row for `Source of truth ingestion` on `/admin/ai` displays estimated cost.
    17. Browser view of `/admin/ai` after the Candidate Designer request shows one usage list row attributed to the tenant, the project, and the workflow `Candidate Designer`.
    18. The usage list row for `Candidate Designer` on `/admin/ai` displays total tokens.
    19. The usage list row for `Candidate Designer` on `/admin/ai` displays estimated cost.
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - Browser activation of the visible `AI governance` action on `/admin/dashboard` opens `/admin/ai`**
    2. **AT2 - Browser submission of a valid model availability change on `/admin/ai` enables one AI model for `Source of truth ingestion`**
    3. **AT3 - Browser submission of a valid model availability change on `/admin/ai` enables one AI model for `Candidate Designer`**
    4. **AT4 - Browser activation of the visible `Upload source material` action on `/tenant/sots` with a tenant-superuser session opens `/tenant/sots/upload`**
    5. **AT5 - Browser submission of one valid text file, topic value, name value, and model value on `/tenant/sots/upload` remains on `/tenant/sots/upload`**
    6. **AT6 - Browser view of `/tenant/sots/upload` after the valid generation request shows one draft source of truth**
    7. **AT7 - Browser submission of a valid generated draft on `/tenant/sots/upload` redirects to `/tenant/sots`**
    8. **AT8 - Browser activation of the visible `Define ICP` action on `/tenant/projects/{projectId}` with the project owner Hiring Manager session opens `/tenant/projects/{projectId}/candidate-designer`**
    9. **AT9 - Browser submission of valid selected sources of truth, model value, and design prompt value on `/tenant/projects/{projectId}/candidate-designer` remains on `/tenant/projects/{projectId}/candidate-designer`**
    10. **AT10 - Browser view of `/tenant/projects/{projectId}/candidate-designer` after the valid generation request shows one draft ICP**
    11. **AT11 - Browser submission of a valid draft ICP on `/tenant/projects/{projectId}/candidate-designer` redirects to `/tenant/projects/{projectId}`**
    12. **AT12 - Browser activation of the visible `View ICP` action on `/tenant/projects/{projectId}` opens `/tenant/projects/{projectId}/icp`**
    13. **AT13 - Browser view of `/tenant/projects/{projectId}/icp` after the saved ICP is opened shows the saved ICP content**
    14. **AT14 - Browser view of `/admin/ai` after the source-of-truth ingestion request shows one usage list row attributed to the tenant and the workflow `Source of truth ingestion`**
    15. **AT15 - Browser view of the usage list row for `Source of truth ingestion` on `/admin/ai` shows total tokens**
    16. **AT16 - Browser view of the usage list row for `Source of truth ingestion` on `/admin/ai` shows estimated cost**
    17. **AT17 - Browser view of `/admin/ai` after the Candidate Designer request shows one usage list row attributed to the tenant, the project, and the workflow `Candidate Designer`**
    18. **AT18 - Browser view of the usage list row for `Candidate Designer` on `/admin/ai` shows total tokens**
    19. **AT19 - Browser view of the usage list row for `Candidate Designer` on `/admin/ai` shows estimated cost**
  - **Explicit Non-Goals (OUT OF SCOPE):** Invoice production. Candidate-facing AI workflows. Multi-project ICP reuse. Non-governed AI execution paths.
  - **Status:** Planned 2026-03-26.
