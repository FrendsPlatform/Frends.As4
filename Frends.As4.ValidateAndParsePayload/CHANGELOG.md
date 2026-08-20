# Changelog

## [1.0.0] - 2026-08-20

### Added

- Initial implementation of the AS4 ValidateAndParsePayload task.
- Validates an incoming AS4 (ebMS) message using the nsoftware IPWorks EDI AS4Server component.
- Extracts the EDI payload, sender/receiver party identifiers and message id.
- Optionally validates the signature (partner certificate) and decrypts the payload (own PFX certificate).
- Returns the AS4 receipt content for use in a later receipt response.
