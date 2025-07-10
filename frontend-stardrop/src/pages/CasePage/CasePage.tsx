'use client'

import { useState } from 'react'
import { useParams } from 'next/navigation'
import { motion } from 'motion/react'

import { CaseLootList, CaseBeforeOpen } from '@/widgets/cases'
import { PageActions } from '@/shared/ui'
import { CaseReopenActions, useOpenCase, useOpenFewCases } from '@/features/cases'
import { CaseRoulette, CaseRouletteVertical, useCase } from '@/entities/cases'

import { useSellItems } from '@/features/inventory'
import { useUser } from '@/shared/hooks'
import type { Loot } from '@/entities/loot'

import classes from './CasePage.module.scss'

type OpenCaseState = 'not-open' | 'opened'

type DroppedLoot = Loot & {
  inventoryId: string
}

export const CasePage = () => {
  const params = useParams<{ id: string }>()
  const caseId = params?.id

  const { user } = useUser()
  const [droppedLootItems, setDroppedLootItems] = useState<DroppedLoot[]>([])
  const [openCaseState, setOpenCaseState] = useState<OpenCaseState>('not-open')
  const [isQuickOpenActive, setQuickOpenActive] = useState(false)
  const [isRouletteAnimationEnd, setRouletteAnimationEnd] = useState(false)
  const [casesToOpenQuantity, setCasesToOpenQuantity] = useState(1)

  const { data: caseData, isLoading: isCaseLoading } = useCase({ id: caseId ?? '' })

  const { mutate: openCase, isPending: isCaseOpening } = useOpenCase({
    onSuccess: ({ droppedLootItem }) => {
      setDroppedLootItems([droppedLootItem])
      setOpenCaseState('opened')
    }
  })

  const { mutate: openFewCases, isPending: isFewCasesOpening } = useOpenFewCases({
    onSuccess: ({ droppedLootItems }) => {
      setDroppedLootItems(droppedLootItems)
      setOpenCaseState('opened')
    }
  })

  const { mutate: sellItems, isPending: isItemsSellingInProcess } = useSellItems()

  const [soldLootItemsInventoryIds, setSoldLootItemsInventoryIds] = useState<string[]>([])

  const handleCaseOpen = (quantity: number) => {
    if (!caseId || !user) return

    setRouletteAnimationEnd(false)
    setSoldLootItemsInventoryIds([])

    if (quantity === 1) {
      openCase({ caseId, userId: user.id })
    } else {
      openFewCases({ caseId, userId: user.id, quantity })
    }
  }

  const handleItemSell = (itemsInventoryIds: string[]) => {
    sellItems(itemsInventoryIds)

    setSoldLootItemsInventoryIds([
      ...soldLootItemsInventoryIds,
      ...itemsInventoryIds.filter((id) => !soldLootItemsInventoryIds.includes(id))
    ])
  }

  const totalItemsSellPrice = droppedLootItems
    .filter(({ inventoryId }) => !soldLootItemsInventoryIds.includes(inventoryId))
    .reduce((prev, curr) => {
      if (!curr?.sellPrice) return prev

      return prev + curr?.sellPrice
    }, 0)

  if (!caseId || !caseData) {
    return
  }

  return (
    <div className={classes.casePage}>
      <PageActions />

      <div className={classes.wrapper}>
        <div className={classes.title}>
          <h1>{caseData.name}</h1>
          <p>Кейс</p>
        </div>

        <div className={classes.caseOpen}>
          {openCaseState === 'not-open' ? (
            <CaseBeforeOpen
              caseData={{
                id: caseId,
                name: caseData.name,
                openPrice: caseData.price,
                image: caseData.image,
                imageType: caseData.type
              }}
              casesToOpenQuantity={casesToOpenQuantity}
              quickOpenActive={isQuickOpenActive}
              isCaseOpening={isCaseOpening || isFewCasesOpening}
              onCasesQuantityChange={setCasesToOpenQuantity}
              onCaseOpen={() => {
                handleCaseOpen(casesToOpenQuantity)
              }}
              onCaseQuickOpen={() => {
                setQuickOpenActive((value) => !value)
              }}
            />
          ) : null}

          {openCaseState === 'opened' && casesToOpenQuantity === 1 ? (
            <div className={classes.caseRoulette}>
              <CaseRoulette
                quickAnimation={isQuickOpenActive}
                droppedItem={droppedLootItems[0]}
                lootItems={caseData.items}
                onAnimationComplete={() => {
                  setRouletteAnimationEnd(true)
                }}
              />
            </div>
          ) : null}

          {openCaseState === 'opened' && casesToOpenQuantity !== 1 ? (
            <div className={classes.caseRouletteVertical}>
              <CaseRouletteVertical
                isAllItemsSold={soldLootItemsInventoryIds.length === droppedLootItems.length}
                quickAnimation={isQuickOpenActive}
                droppedItems={droppedLootItems}
                lootItems={caseData.items}
                onAnimationComplete={() => {
                  setRouletteAnimationEnd(true)
                }}
                onItemSell={(itemInentoryId) => {
                  handleItemSell([itemInentoryId])
                }}
              />
            </div>
          ) : null}

          {openCaseState === 'opened' ? (
            <motion.div
              className={classes.caseReopenAcitons}
              initial={{ opacity: 0, visibility: 'hidden', y: 100 }}
              animate={
                isRouletteAnimationEnd
                  ? {
                      opacity: 1,
                      visibility: 'visible',
                      y: 0
                    }
                  : undefined
              }
              transition={{
                duration: 0.5,
                ease: 'easeOut'
              }}
            >
              <CaseReopenActions
                itemsSellPrice={totalItemsSellPrice}
                onCaseReopen={() => {
                  setOpenCaseState('not-open')
                }}
                onAllItemsSell={() => {
                  const allItemsInventoryIds = droppedLootItems.map(
                    ({ inventoryId }) => inventoryId
                  )
                  handleItemSell(allItemsInventoryIds)
                }}
                onItemUpgrade={() => {}}
              />
            </motion.div>
          ) : null}
        </div>

        <section className={classes.caseLoot}>
          <h2>Содержимое кейса</h2>

          <div className={classes.caseLootList}>
            {caseData ? <CaseLootList loot={caseData.items} /> : null}
          </div>
        </section>
      </div>
    </div>
  )
}
