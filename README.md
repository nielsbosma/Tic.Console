# Tic.Console

CLI for the [TIC](https://tic.io) search and datasets API — company, person, vehicle, and bankruptcy lookup. YAML-first output optimized for LLM agent consumption.

## Installation

```bash
dotnet tool install -g Tic.Console
```

## Authentication

Set your API key via environment variable:

```bash
export TIC_API_KEY=your-api-key
```

Or pass it per command:

```bash
tic company get 556792-6687 --api-key your-api-key
```

## Commands

### Company

```bash
# Look up by registration number
tic company get 556792-6687

# Look up by internal TIC ID
tic company get-by-id 3325421

# Search by name, phone, email, address, etc.
tic company search "Bosma Interactive"
tic company search "+46767742725" --query-by "phoneNumbers.e164PhoneNumber"
tic company search "niels@example.com" --query-by "emailAddresses.emailAddress"
tic company search "Fabriksgatan" --query-by "mostRecentRegisteredAddress.streetAddress"
tic company search "*" --query-by "registrationNumber" --filter-by "hasIntelligence:true" --per-page 5

# Detailed lookups by company ID
tic company parties 3325421
tic company beneficial-owners 3325421
tic company intelligence 3325421
tic company graph 3325421
tic company tree 3325421
tic company debtor-summary 3325421
tic company vehicles 3325421
tic company properties 3325421
```

### Person

```bash
# Search by personal identity number
tic person search 198207174171

# Get person details
tic person get 1625054

# Get all companies where a person has a role
tic person companies 1625054
```

### Vehicle

```bash
# Search by licence plate, VIN, manufacturer, etc.
tic vehicle search "ABC123"
tic vehicle search "YV1XZ" --query-by vin
```

### Bankruptcy

```bash
# Search bankruptcy records
tic bankruptcy search "5566778899"
tic bankruptcy search "*" --filter-by "initiatedDate:>=1711929600" --sort-by "initiatedDate:desc"
```

### Output formats

```bash
# YAML (default)
tic company get 556792-6687

# JSON
tic company get 556792-6687 --format json

# Table
tic company get 556792-6687 --format table
```

### Verbose mode

```bash
tic company get 556792-6687 --verbose
```

## Exit codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 1 | User/input error |
| 2 | API/network error |

## License

MIT
