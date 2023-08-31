export class DateManager {
    compareDates(date1: Date, date2: Date): number {
        if (date1.getFullYear() !== date2.getFullYear()) {
            return date1.getFullYear() - date2.getFullYear();
        }
    
        if (date1.getMonth() !== date2.getMonth()) {
            return date1.getMonth() - date2.getMonth();
        }
    
        return date1.getDate() - date2.getDate();
    }
}