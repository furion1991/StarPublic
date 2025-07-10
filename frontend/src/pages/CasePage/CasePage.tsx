'use client'

import { useState, useMemo } from 'react'
import { useParams } from 'next/navigation'
import { motion } from 'motion/react'

import { CaseLootList, CaseBeforeOpen } from '@/widgets/cases'
import { PageActions } from '@/shared/ui'
import { CaseReopenActions, useOpenCase } from '@/features/cases'

import { CaseRoulette, useCase } from '@/entities/cases'
import { useUser } from '@/shared/hooks'
import { useCaseItem } from '@/entities/loot'

import classes from './CasePage.module.scss'

type OpenCaseState = 'not-open' | 'opened'

export const CasePage = () => {
  const params = useParams<{ id: string }>()
  const caseId = params?.id

  const { user } = useUser()
  const [droppedLootItemId, setDroppedLootItemId] = useState('')
  const [openCaseState, setOpenCaseState] = useState<OpenCaseState>('not-open')
  const [isQuickOpenActive, setQuickOpenActive] = useState(false)
  const [isRouletteAnimationEnd, setRouletteAnimationEnd] = useState(false)

  const { data: caseData, isLoading: isCaseLoading } = useCase({ id: caseId ?? '' })

  const { mutate: openCase, isPending: isCaseOpening } = useOpenCase({
    onSuccess: ({ droppedLootItemId }) => {
      setDroppedLootItemId(droppedLootItemId)
      setOpenCaseState('opened')
    }
  })

  const { data: droppedLootItem, isLoading: isDroppedLootItemLoading } = useCaseItem({
    itemId: droppedLootItemId
  })

  const handleCaseOpen = () => {
    if (!caseId || !user) return

    setRouletteAnimationEnd(false)
    openCase({ caseId, userId: user.id })
  }

  const memoizedCaseRoulette = useMemo(() => {
    if (!caseData) return

    return openCaseState === 'opened' ? (
      <div className={classes.caseRoulette}>
        <CaseRoulette
          quickOpen={isQuickOpenActive}
          droppedItemId={droppedLootItemId}
          lootItems={caseData.items}
          onAnimationComplete={() => {
            setRouletteAnimationEnd(true)
          }}
        />
      </div>
    ) : null
  }, [isQuickOpenActive, droppedLootItemId, openCaseState, caseData])

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
              quickOpenActive={isQuickOpenActive}
              isCaseOpening={isCaseOpening}
              onCaseOpen={handleCaseOpen}
              onCaseQuickOpen={() => {
                setQuickOpenActive((value) => !value)
              }}
            />
          ) : null}

          {memoizedCaseRoulette}

          {droppedLootItem && openCaseState === 'opened' ? (
            <motion.div
              className={classes.caseReopenAcitons}
              initial={{ opacity: 0, y: 100 }}
              animate={
                isRouletteAnimationEnd
                  ? {
                      opacity: 1,
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
                itemSellPrice={droppedLootItem.sellPrice}
                onCaseReopen={() => {
                  setOpenCaseState('not-open')
                }}
                onItemSell={() => {}}
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
