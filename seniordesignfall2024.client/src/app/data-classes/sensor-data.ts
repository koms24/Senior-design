
export class SensorData {
  public name!: string;
  public value!: any;

  public constructor(_name: string, _val: any) {
    this.name = _name;
    this.value = _val;
  }

  public IsInRange(): boolean {
    return (this.value > 0.5);
  }
}
