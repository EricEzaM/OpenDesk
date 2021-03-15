export default interface Desk
{
  id: string
  name: string
  diagramPosition: DiagramPosition
}

interface DiagramPosition {
  x: number
  y: number
}