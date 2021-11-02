# Ethos Training

This application for [Ethos Training](https://ethostraining.it) ([Instagram](https://www.instagram.com/ethos.training)) allow trainers to create schedules (single or recurring) that will be later booked by customers. 

## Use cases for v1

### Login & user management

- Allow users to register, login and reset password

### Create schedule

- Create a single schedule
- Create a recurring schedule

### Delete schedule

- Delete a single schedule
- Delete a recurring schedule
    - Instance
    - Instance and all future (deleting the first instance completely deletes the recurring schedule)

### Update schedule

- Update single schedule
    - Update single
    - Convert to recurring
- Update recurring
    - Update instance
    - Update instance and future (if first instance update all)
    - Convert to single (delete instance and future, then create a new single)

### Booking
  - Create a booking for a specific date/time
  - Delete a booking
