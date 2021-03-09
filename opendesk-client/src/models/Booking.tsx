import User from "./User";

export default interface Booking
{
  id: string
  deskId: string
  startDateTime: Date
  endDateTime: Date
  user: User
}