export class DateManager {

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = date.getMonth();
    const day = date.getDay();
    const hours = date.getHours();
    const minutes = date.getMinutes();
    const seconds = date.getSeconds();

    return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
  }
}