import { LootRarity } from '../types/loot.types'

export type Loot = {
  id: string
  name: string
  type: number
  rarity: LootRarity
  baseCost: number
  sellPrice: number
  isVisible: boolean
  game: string
  image: string
}

export type BestDropResponse = {
  count: number | null
  message: string
  page: number | null
  statusCode: string
  result: Loot
}

export type GetAllItemsProps = {
  valueFrom: number
  nameSearch: string
}
