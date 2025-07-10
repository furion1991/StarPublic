import { LootRarity } from '../types/loot.types'

export type Item = {
  id: string
  name: string
  type: number
  rarity: LootRarity
  baseCost: number
  sellPrice: number
  isVisible: boolean
  dropChange: null
  game: string
  image: string
}

export type BestDropResponse = {
  count: number | null
  message: string
  page: number | null
  statusCode: string
  result: Item
}
