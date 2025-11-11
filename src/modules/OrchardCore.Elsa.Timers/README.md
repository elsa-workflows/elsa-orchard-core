# Elsa.OrchardCore.Timers

Provides timer services and scheduling activities for Elsa workflows in Orchard Core.

## Features

This module adds time-based scheduling and timer capabilities to Elsa workflows.

### Timer Services

Two implementations are provided:

#### 1. Timer Services and Activities (Base)

- **Cron-based Scheduling** - Schedule workflows using cron expressions
- **Delay Activities** - Add time delays in workflow execution
- **Timer Triggers** - Start workflows based on time conditions
- **Time-based Conditions** - Conditional logic based on dates and times

#### 2. Quartz Timer Provider (Optional)

An alternative timer implementation using Quartz.NET:

- **Clustered Deployment Support** - Designed for multi-server environments
- **Distributed Job Scheduling** - Coordinated scheduling across cluster nodes
- **Persistent Schedule Storage** - Schedules survive application restarts
- **Reliable Execution** - Guaranteed job execution in distributed scenarios

## Use Cases

- Scheduled content publishing
- Periodic data synchronization
- Timed notifications and alerts
- Batch processing at specific intervals
- Maintenance and cleanup tasks
- Time-based workflow triggers

## Features

### Timer Services
- **OrchardCore.Elsa.Timers** - Base timer services and activities

### Quartz Provider
- **OrchardCore.Elsa.Timers.Quartz** - Quartz.NET-based implementation for clustered deployments

## When to Use Quartz

Enable the Quartz Timer Provider if:

- Running in a clustered/load-balanced environment
- Need guaranteed job execution across servers
- Require persistent schedule storage
- Want advanced scheduling features

## Dependencies

- `OrchardCore.Elsa`

## Installation

1. Enable the **Timer Services and Activities** feature for basic timer functionality
2. Optionally enable **Quartz Timer Provider** for clustered deployments

## Package Information

- **Package ID**: `Elsa.OrchardCore.Timers`
- **Category**: Elsa
