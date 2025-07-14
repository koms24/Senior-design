
export enum ControlState {
  UNKOWN = 0,
  ON = 1,
  OFF = 2
}

export class ControlData {
  public name: string;
  public state: ControlState | null;
  
  public constructor(_name: string, _state: ControlState | null) {
    this.name = _name;
    this.state = _state;
  }
}

