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

- [ ] **SIP-2.1: Committed data changes create transaction audit records**
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
  - **Status:** Planned 2026-03-24.

- [ ] **SIP-2.2: Governed records expose compliance metadata**
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
  - **Status:** Planned 2026-03-24.

- [ ] **SIP-2.3: Data-subject requests can be recorded and state-tracked**
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
  - **Status:** Planned 2026-03-24.

- [ ] **SIP-2.4: Compliance contracts are schema-verified**
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
  - **Status:** Planned 2026-03-24.
