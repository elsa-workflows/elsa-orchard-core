# Elsa.OrchardCore.UI

Provides UI-related workflow activities for Orchard Core.

## Features

This module adds user interface interaction capabilities to Elsa workflows, enabling workflows to communicate with users through the Orchard Core UI.

### UI Activities

**DisplayNotification** - Display notifications to users in the Orchard Core admin interface

This activity allows workflows to:
- Show success, information, warning, or error messages
- Provide feedback on workflow execution
- Communicate status updates to users
- Alert administrators to important events

### Notification Types

The module supports various notification styles:

- **Success** - Confirm successful operations
- **Information** - Provide informational messages
- **Warning** - Alert users to potential issues
- **Error** - Report errors and failures

## Use Cases

- User feedback on content operations
- Workflow status notifications
- Error reporting and alerts
- Process completion confirmations
- Administrative notifications

## Dependencies

- `OrchardCore.Elsa`

## Installation

Enable the **UI Activities** feature in the Orchard Core admin dashboard under Features.

## Package Information

- **Package ID**: `Elsa.OrchardCore.UI`
- **Category**: Elsa
